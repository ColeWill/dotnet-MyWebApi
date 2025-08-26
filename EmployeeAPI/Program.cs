using System.ComponentModel.DataAnnotations;
using EmployeeAPI;
using EmployeeAPI.Abstractions;
using Microsoft.AspNetCore.Mvc;

public class Program
{
    private static void Main(string[] args)
    {
        var employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                SocialSecurityNumber = "123456",
                Address1 = "123 Main st",
                Address2 = "Unit #10",
                City = "Aptos",
                State = "CA",
                ZipCode = "95017",
                PhoneNumber = "123456",
                Email = "test@test.com",
            },
            new Employee
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Doe",
                SocialSecurityNumber = "123456",
                Address1 = "123 Main st",
                Address2 = "Unit #10",
                City = "Aptos",
                State = "CA",
                ZipCode = "95017",
                PhoneNumber = "123456",
                Email = "test@test.com",
            },
        };

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IRepository<Employee>, EmployeeRepository>();
        builder.Services.AddProblemDetails();

        var app = builder.Build();
        // Seed the repository
        var repository = app.Services.GetRequiredService<IRepository<Employee>>();
        foreach (var employee in employees)
        {
            repository.Create(employee);
        }
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        // App Routes *********************************
        var employeeRoute = app.MapGroup("/employees");

        employeeRoute.MapGet(
            string.Empty,
            (IRepository<Employee> repository) =>
            {
                return Results.Ok(
                    repository
                        .GetAll()
                        .Select(employee => new GetEmployeeResponse
                        {
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,

                            Address1 = employee.Address1,
                            Address2 = employee.Address2,
                            City = employee.City,
                            State = employee.State,
                            ZipCode = employee.ZipCode,
                            PhoneNumber = employee.PhoneNumber,
                            Email = employee.Email,
                        })
                );
            }
        );

        employeeRoute.MapGet(
            "{id:int}",
            (int id, IRepository<Employee> repository) =>
            {
                var employee = repository.GetById(id);
                if (employee == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(
                    new GetEmployeeResponse
                    {
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,

                        Address1 = employee.Address1,
                        Address2 = employee.Address2,
                        City = employee.City,
                        State = employee.State,
                        ZipCode = employee.ZipCode,
                        PhoneNumber = employee.PhoneNumber,
                        Email = employee.Email,
                    }
                );
            }
        );

        employeeRoute.MapPost(
            string.Empty,
            ([FromBody] CreateEmployeeRequest employeeRequest, IRepository<Employee> repository) =>
            {
                var validationProblems = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(
                    employeeRequest,
                    new ValidationContext(employeeRequest),
                    validationProblems,
                    true
                );
                if (!isValid)
                {
                    return Results.BadRequest(validationProblems.ToValidationProblemDetails());
                }

                var newEmployee = new Employee
                {
                    Id = repository.GetAll().Max(e => e.Id) + 1,
                    FirstName = employeeRequest.FirstName!,
                    LastName = employeeRequest.LastName!,
                    SocialSecurityNumber = employeeRequest.SocialSecurityNumber!,
                    Address1 = employeeRequest.Address1,
                    Address2 = employeeRequest.Address2,
                    City = employeeRequest.City,
                    State = employeeRequest.State,
                    ZipCode = employeeRequest.ZipCode,
                    PhoneNumber = employeeRequest.PhoneNumber,
                    Email = employeeRequest.Email,
                };
                repository.Create(newEmployee);
                return Results.Created($"/employees/{newEmployee.Id}", employeeRequest);
            }
        );

        employeeRoute.MapPut(
            "{id}",
            (UpdateEmployeeRequest employeeRequest, int id, IRepository<Employee> repository) =>
            {
                var existingEmployee = repository.GetById(id);
                if (existingEmployee == null)
                {
                    return Results.NotFound();
                }

                existingEmployee.Address1 = employeeRequest.Address1;
                existingEmployee.Address2 = employeeRequest.Address2;
                existingEmployee.City = employeeRequest.City;
                existingEmployee.State = employeeRequest.State;
                existingEmployee.ZipCode = employeeRequest.ZipCode;
                existingEmployee.PhoneNumber = employeeRequest.PhoneNumber;
                existingEmployee.Email = employeeRequest.Email;

                return Results.Ok(existingEmployee);
            }
        );

        app.UseHttpsRedirection();

        app.Run();
    }
}
