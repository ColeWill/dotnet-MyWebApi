var builder = WebApplication.CreateBuilder(args);
var employees = new List<Employee>
{
    new Employee
    {
        Id = 1,
        FirstName = "John",
        LastName = "Doe",
    },
    new Employee
    {
        Id = 2,
        FirstName = "Jane",
        LastName = "Doe",
    },
};

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var employeeRoute = app.MapGroup("employees");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

employeeRoute.MapGet(
    string.Empty,
    () =>
    {
        return Results.Ok(employees);
    }
).WithName("GetEmployees")
.WithOpenApi(operation => new(operation)
{
    Summary = "Get all employees",
    Description = "Retrieves a list of all employees"
});

employeeRoute.MapGet(
    "/{id:int}",
    (int id) =>
    {
        var employee = employees.SingleOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(employee);
    }
).WithName("GetEmployeeById")
.WithOpenApi(operation => new(operation)
{
    Summary = "Get employee by ID",
    Description = "Retrieves a specific employee by their ID"
});

employeeRoute.MapPost(
    string.Empty,
    (Employee employee) =>
    {
        employee.Id = employees.Max(e => e.Id) + 1;
        employees.Add(employee);
        return Results.Created($"employees/{employee.Id}", employee);
    }
).WithName("CreateEmployee")
.WithOpenApi(operation => new(operation)
{
    Summary = "Create new employee",
    Description = "Creates a new employee record"
});

app.Run();
