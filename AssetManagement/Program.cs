using AssetManagementApi.Data;
using AssetManagementApi.Dtos.Assets;
using AssetManagementApi.Dtos.Employees;
using AssetManagementApi.Dtos.SoftwareLicenses;
using AssetManagementApi.Dtos.Status;
using AssetManagementApi.Mappers;
using AssetManagementApi.Middleware;
using AssetManagementApi.Repositories;
using AssetManagementApi.Services;
using AssetManagementApi.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AssetManagementContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IValidator<AssetCreateDto>, AssetCreateValidator>();
builder.Services.AddScoped<IValidator<AssetUpdateDto>, AssetUpdateValidator>();
builder.Services.AddScoped<IValidator<EmployeeCreateUpdateDto>, EmployeeCreateUpdateValidator>();
builder.Services.AddScoped<IValidator<SoftwareLicenseCreateUpdateDto>, SoftwareLicenseCreateUpdateValidator>();
builder.Services.AddScoped<IValidator<StatusCreateUpdateDto>, StatusCreateUpdateValidator>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ISoftwareLicenseRepository, SoftwareLicenseRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISoftwareLicenseService, SoftwareLicenseService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IAssetMapper, AssetMapper>();
builder.Services.AddScoped<IEmployeeMapper, EmployeeMapper>();
builder.Services.AddScoped<ISoftwareLicenseMapper, SoftwareLicenseMapper>();
builder.Services.AddScoped<IStatusMapper, StatusMapper>();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AssetManagementContext>();
    try
    {
        await dbContext.Database.MigrateAsync();
    }
    catch (InvalidOperationException) when (app.Environment.IsDevelopment())
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AssetManagement API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();