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
        Id = 1,
        FirstName = "Jane",
        LastName = "Doe",
    },
};

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use(
    async (HttpContext context, RequestDelegate next) =>
    {
        context.Response.Headers.Append("Content-Type", "text/html");
        await context.Response.WriteAsync("<h1>Welcome to Ahoy! </h1>");
    }
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet(
    "/employees",
    () =>
    {
        return employees;
    }
);

app.Run();
