using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using EmployeeAPI.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TheEmployeeAPI.Tests;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;

        var repo = _factory.Services.GetRequiredService<IRepository<Employee>>();
        repo.Create(
            new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                SocialSecurityNumber = "111-11-11111",
            }
        );
    }

    [Fact]
    public async Task GetAllEmployees_ReturnsOKResponse()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/employees");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/employees/1");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateEmployee_ReturnsCreatedResult()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/employees",
            new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                SocialSecurityNumber = "23423432",
            }
        );

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateEmployee_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            "/employees/1",
            new Employee
            {
                FirstName = "FirstName",
                LastName = "LastName",
                SocialSecurityNumber = "1111111",
            }
        );
    }

    [Fact]
    public async Task CreateEmployee_ReturnsRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidEmployee = new CreateEmployeeRequest();

        // Act
        var response = await client.PostAsJsonAsync("/employees", invalidEmployee);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("FirstName", problemDetails.Errors.Keys);
        Assert.Contains("LastName", problemDetails.Errors.Keys);
        Assert.Contains("The FirstName field is required.", problemDetails.Errors["FirstName"]);
        Assert.Contains("The LastName field is required.", problemDetails.Errors["LastName"]);
    }

    [Fact]
    public async Task UpdateEmployee_ReturnsNotFoundForNonExistentEmployee()
    {
        var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            "/employees/999999",
            new Employee
            {
                FirstName = "FirstName",
                LastName = "LastName",
                SocialSecurityNumber = "1111111",
            }
        );
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
