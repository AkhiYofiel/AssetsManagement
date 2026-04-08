using AssetManagementApi.Models;
using AssetManagementApi.Repositories;
using Xunit;

namespace AssetManagement.Tests.Repositories;

public class SoftwareLicenseRepositoryTests
{
    [Fact]
    public async Task GetDetailAsync_ReturnsLicenseWithAssets()
    {
        await using var context = TestDbContextFactory.CreateContext();

        var status = new Status { Id = 1, Name = "Assigned" };
        var asset = new Asset { Id = 2, Name = "Laptop", SerialNumber = "SN001", StatusId = status.Id };
        var license = new SoftwareLicense { Id = 3, Name = "MSOffice", LicenseKey = "ABC123", ExpirationDate = DateTime.UtcNow.AddDays(30) };
        var link = new AssetSoftwareLicense { AssetId = asset.Id, SoftwareLicenseId = license.Id };

        context.Statuses.Add(status);
        context.Assets.Add(asset);
        context.SoftwareLicenses.Add(license);
        context.AssetSoftwareLicenses.Add(link);
        await context.SaveChangesAsync();

        var repository = new SoftwareLicenseRepository(context);

        var result = await repository.GetDetailAsync(license.Id);

        Assert.NotNull(result);
        Assert.Single(result!.AssetSoftwareLicenses);
        Assert.NotNull(result.AssetSoftwareLicenses.First().Asset);
        Assert.NotNull(result.AssetSoftwareLicenses.First().Asset!.Status);
    }

    [Fact]
    public async Task GetDetailAsync_WhenMissing_ReturnsNull()
    {
        await using var context = TestDbContextFactory.CreateContext();
        var repository = new SoftwareLicenseRepository(context);

        var license = await repository.GetDetailAsync(999);

        Assert.Null(license);
    }
}
