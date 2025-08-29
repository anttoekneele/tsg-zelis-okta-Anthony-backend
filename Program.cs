using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS (for communication with Blazor Server UI)
var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSeveralSpecificOrigins", policy =>
    {
        policy
            .AllowAnyOrigin() // remove when frontend is created
                              // .WithOrigins("allowedOrigins") <-- uncomment and update url when frontend is created
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configure SQL Server connection
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register the Audit Service
builder.Services.AddScoped<IAuditService, AuditService>();

// Register Role Service
builder.Services.AddScoped<IRoleService, RoleService>();

// Configure GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType(d => d.Name("Query"))
        .AddType<UserQueries>()
        .AddType<RoleQueries>()
        .AddType<SecurityEventQueries>()
    .AddMutationType(d => d.Name("Mutation"))
        .AddType<RoleMutations>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Map HTTP requests to GraphQL endpoint
app.MapGraphQL();

// Manual Test
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var roleService = services.GetRequiredService<IRoleService>();
    var auditService = services.GetRequiredService<IAuditService>();

    var userId = Guid.Parse("5ffd7f7f-52f9-45f4-be38-3f3f10456063");
    var roleId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // BasicUser
    var authorId = userId;

    // ✅ Add a test user
    // context.Users.Add(new User
    // {
    //     Id = userId,
    //     Email = "test@example.com",
    //     ExternalId = "external-id",
    //     RoleId = roleId
    // });
    // await context.SaveChangesAsync();
    // Console.WriteLine("✅ User added");

    // ✅ Test role assignment
    // await roleService.AssignRole(userId, roleId, authorId);
    // Console.WriteLine("✅ Role assignment test complete");

    // ✅ Test login event
    // await auditService.LogLoginEvent(userId, "test-provider");
    // Console.WriteLine("✅ Login event logged");

    // ✅ Test logout event
    // await auditService.LogLogoutEvent(userId);
    // Console.WriteLine("✅ Logout event logged");

    // ✅ Query all security events
    var events = await context.SecurityEvents
        .Where(e => e.AuthorUserId == userId)
        .OrderByDescending(e => e.OccurredUtc)
        .ToListAsync();

    Console.WriteLine("📄 Security Events:");
    foreach (var e in events)
    {
        Console.WriteLine($"- {e.EventType} | {e.OccurredUtc} | {e.Details}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}