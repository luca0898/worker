using Application.UseCases;
using Database;
using Domain.Contracts;
using Worker.BackgroundServices;

namespace Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddHealthChecks();

        builder.Services.AddControllers();

        builder.Services.Configure<ConsumersConfigurations>(builder.Configuration.GetSection(nameof(ConsumersConfigurations)));

        builder.Services.AddInfraDatabase(builder.Configuration);

        builder.Services.AddScoped<ICreateMaterialUseCase, CreateMaterialUseCase>();

        builder.Services.AddHostedService<MaterialsConsumer>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseHealthChecks("/health");

        app.MapControllers();

        app.Run();
    }
}
