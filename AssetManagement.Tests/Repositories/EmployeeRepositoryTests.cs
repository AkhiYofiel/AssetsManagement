using AssetManagementApi.Models;
using AssetManagementApi.Repositories;
using Xunit;

namespace AssetManagement.Tests.Repositories;

public class EmployeeRepositoryTests
{
    [Fact]
    public async Task GetDetailAsync_ReturnsEmployeeWithAssetsAndStatuses()
    {
        await using var context = TestDbContextFactory.CreateContext();

        var status = new Status { Id = 1, Name = "Assigned" };
        var employee = new Employee { Id = 2, FirstName = "Neom", LastName = "Omy", Email = "example@example.com" };
        var asset = new Asset { Id = 3, Name = "Laptop", SerialNumber = "SN001", StatusId = status.Id, EmployeeId = employee.Id };

        context.Statuses.Add(status);
        context.Employees.Add(employee);
        context.Assets.Add(asset);
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);

        var result = await repository.GetDetailAsync(employee.Id);

        Assert.NotNull(result);
        Assert.Single(result!.Assets);
        Assert.NotNull(result.Assets.First().Status);
    }

    [Fact]
    public async Task GetDetailAsync_WhenMissing_ReturnsNull()
    {
        await using var context = TestDbContextFactory.CreateContext();
        var repository = new EmployeeRepository(context);

        var employee = await repository.GetDetailAsync(999);

        Assert.Null(employee);
    }
}
