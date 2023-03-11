namespace LicenseServer;

using System.IO.Abstractions;
using System.Text.Json.Serialization;
using LicenseServer;
using Microsoft.AspNetCore.Http.Json;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddLogging();
        builder.Services.AddSingleton<ILicenseRepository, LicenseRepository>();
        builder.Services.AddSingleton<IFileSystem, FileSystem>();
        builder.Services.AddTransient<ILicenseService, LicenseService>();
        builder.Services.AddControllers();
        builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        await InitializeRepository(app);

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static async Task InitializeRepository(WebApplication app)
    {
        var repository = app.Services.GetService<ILicenseRepository>();
        await repository.InitializeAsync();
    }
}
