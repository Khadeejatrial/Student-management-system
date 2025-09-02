using Microsoft.EntityFrameworkCore;
using SMS.Infrastructure;
using SMS.Services.Interface;
using SMS.Services.Implementations;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog for startup errors
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext()
                     .WriteTo.Console());

    // Add DbContext
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



    // Add Services
    builder.Services.AddScoped<IStudentApplication, StudentApplication>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // Log startup errors using Serilog
    Log.Fatal(ex, "Application failed to start correctly.");
}
finally
{
    // Ensure logs are flushed before application exits
    Log.CloseAndFlush();
}
