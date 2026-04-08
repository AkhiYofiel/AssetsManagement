using AssetManagementApi.Models;
using AssetManagementApi.Repositories;
using Xunit;

namespace AssetManagement.Tests.Repositories;

public class AssetRepositoryTests
{
    [Fact]
    public async Task GetDetailAsync_ReturnsData()
    {
        await using var context = TestDbContextFactory.CreateContext();

        var status = new Status { Id = 1, Name = "Assigned" };
        var employee = new Employee { Id = 2, FirstName = "Test", LastName = "M", Email = "example@example.com" };
        var license = new SoftwareLicense { Id = 3, Name = "MSOffice", LicenseKey = "ABC123", ExpirationDate = DateTime.UtcNow.AddDays(30) };
        var asset = new Asset { Id = 4, Name = "Laptop", SerialNumber = "SN001", StatusId = status.Id, EmployeeId = employee.Id };
        var warranty = new WarrantyCard { Id = 5, Provider = "Dell", StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddYears(1), AssetId = asset.Id };
        var link = new AssetSoftwareLicense { AssetId = asset.Id, SoftwareLicenseId = license.Id };

        context.Statuses.Add(status);
        context.Employees.Add(employee);
        context.SoftwareLicenses.Add(license);
        context.Assets.Add(asset);
        context.WarrantyCards.Add(warranty);
        context.AssetSoftwareLicenses.Add(link);
        await context.SaveChangesAsync();

        var repository = new AssetRepository(context);
        var detail = await repository.GetDetailAsync(asset.Id);

        Assert.NotNull(detail);
        Assert.NotNull(detail!.Status);
        Assert.NotNull(detail.Employee);
        Assert.NotNull(detail.WarrantyCard);
        Assert.Single(detail.AssetSoftwareLicenses);
    }

    [Fact]
    public async Task StatusExistsAsync_ReturnsExpectedResult()
    {
        await using var context = TestDbContextFactory.CreateContext();
        context.Statuses.Add(new Status { Id = 1, Name = "Active" });
        await context.SaveChangesAsync();

        var repository = new AssetRepository(context);

        var exists = await repository.StatusExistsAsync(1);
        var missing = await repository.StatusExistsAsync(999);

        Assert.True(exists);
        Assert.False(missing);
    }

    [Fact]
    public async Task GetForUpdateAsync_IncludesWarrantyCard()
    {
        await using var context = TestDbContextFactory.CreateContext();

        var status = new Status { Id = 1, Name = "Assigned" };
        var asset = new Asset { Id = 2, Name = "Laptop", SerialNumber = "SN001", StatusId = status.Id };
        var warranty = new WarrantyCard { Id = 3, Provider = "Dell", StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddYears(1), AssetId = asset.Id };

        context.Statuses.Add(status);
        context.Assets.Add(asset);
        context.WarrantyCards.Add(warranty);
        await context.SaveChangesAsync();

        var repository = new AssetRepository(context);
        var result = await repository.GetForUpdateAsync(asset.Id);

        Assert.NotNull(result);
        Assert.NotNull(result!.WarrantyCard);
    }

    [Fact]
    public async Task GetForDeleteAsync_IncludesRelatedCollections()
    {
        await using var context = TestDbContextFactory.CreateContext();

        var status = new Status { Id = 1, Name = "Assigned" };
        var asset = new Asset { Id = 2, Name = "Laptop", SerialNumber = "SN001", StatusId = status.Id };
        var warranty = new WarrantyCard { Id = 3, Provider = "Dell", StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddYears(1), AssetId = asset.Id };
        var license = new SoftwareLicense { Id = 4, Name = "MSOffice", LicenseKey = "ABC123", ExpirationDate = DateTime.UtcNow.AddDays(30) };
        var link = new AssetSoftwareLicense { AssetId = asset.Id, SoftwareLicenseId = license.Id };

        context.Statuses.Add(status);
        context.Assets.Add(asset);
        context.WarrantyCards.Add(warranty);
        context.SoftwareLicenses.Add(license);
        context.AssetSoftwareLicenses.Add(link);
        await context.SaveChangesAsync();

        var repository = new AssetRepository(context);
        var result = await repository.GetForDeleteAsync(asset.Id);

        Assert.NotNull(result);
        Assert.NotNull(result!.WarrantyCard);
        Assert.Single(result.AssetSoftwareLicenses);
    }

    [Fact]
    public async Task EmployeeAssetAndLicenseExistsAsync_ReturnExpectedResults()
    {
        await using var context = TestDbContextFactory.CreateContext();

        var status = new Status { Id = 1, Name = "Assigned" };
        var employee = new Employee { Id = 2, FirstName = "Hasmi", LastName = "K", Email = "test@test.com" };
        var asset = new Asset { Id = 3, Name = "Laptop", SerialNumber = "SN001", StatusId = status.Id, EmployeeId = employee.Id };
        var license = new SoftwareLicense { Id = 4, Name = "MSOffice", LicenseKey = "ABC123", ExpirationDate = DateTime.UtcNow.AddDays(30) };

        context.Statuses.Add(status);
        context.Employees.Add(employee);
        context.Assets.Add(asset);
        context.SoftwareLicenses.Add(license);
        await context.SaveChangesAsync();

        var repository = new AssetRepository(context);

        Assert.True(await repository.EmployeeExistsAsync(employee.Id));
        Assert.True(await repository.AssetExistsAsync(asset.Id));
        Assert.True(await repository.SoftwareLicenseExistsAsync(license.Id));

        Assert.False(await repository.EmployeeExistsAsync(999));
        Assert.False(await repository.AssetExistsAsync(999));
        Assert.False(await repository.SoftwareLicenseExistsAsync(999));
    }

    [Fact]
    public async Task AddFindAndRemoveAssetSoftwareLicense_WorksAsExpected()
    {
        await using var context = TestDbContextFactory.CreateContext();

        var status = new Status { Id = 1, Name = "Assigned" };
        var asset = new Asset { Id = 2, Name = "Laptop", SerialNumber = "SN001", StatusId = status.Id };
        var license = new SoftwareLicense { Id = 3, Name = "MSOffice", LicenseKey = "ABC123", ExpirationDate = DateTime.UtcNow.AddDays(30) };

        context.Statuses.Add(status);
        context.Assets.Add(asset);
        context.SoftwareLicenses.Add(license);
        await context.SaveChangesAsync();

        var repository = new AssetRepository(context);
        var link = new AssetSoftwareLicense { AssetId = asset.Id, SoftwareLicenseId = license.Id };

        await repository.AddAssetSoftwareLicenseAsync(link);
        await repository.SaveChangesAsync();

        var fromDb = await repository.FindAssetSoftwareLicenseAsync(asset.Id, license.Id);
        Assert.NotNull(fromDb);

        await repository.RemoveAssetSoftwareLicenseAsync(link);
        await repository.SaveChangesAsync();

        var removed = await repository.FindAssetSoftwareLicenseAsync(asset.Id, license.Id);
        Assert.Null(removed);
    }
}
