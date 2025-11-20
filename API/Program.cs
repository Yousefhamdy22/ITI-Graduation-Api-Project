using API.Handlers;
using Application;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DataBase");

builder.Services.AddInfrastructureDependencies().AddServiceDependencies()
           .AddServiceRegisteration(builder.Configuration);

builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(connectionString, b =>
        b.MigrationsAssembly(typeof(AppDBContext).Assembly.FullName));
});


#region Swagger Config

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Learning API",
        Version = "v1",
        Description = "API for E-Learning Platform"
    });

    // Add JWT Bearer authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });


    try
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    }
    catch
    {
        // Ignore if XML comments aren't available
    }
});
#endregion



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.SeedRolesAsync();
}
#region Swagger Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Learning API V1");
    c.RoutePrefix = "swagger"; // This makes it available at /swagger

    // For production, you might want to hide the Swagger UI
    // but keep the JSON available for API consumers
    if (!app.Environment.IsDevelopment())
    {
        c.DocumentTitle = "API Documentation - Production";
    }
});

#endregion

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

