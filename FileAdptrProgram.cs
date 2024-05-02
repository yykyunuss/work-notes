using Fctr.Edison.FileAdapter.DataAccess;
using Fctr.Edison.FileAdapter.Interfaces;
using Fctr.Edison.FileAdapter.Repositories;
using Fctr.Edison.FileAdapter.Repositories.Models;
using Fctr.Edison.FileAdapter.Services;
using Fctr.Edison.FileAdapter.Settings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Fctr.Edison.FileAdapter
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((hostContext, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName();
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                Console.WriteLine($"Application name: {context.HostingEnvironment.ApplicationName}");
                Console.WriteLine($"Application env: {context.HostingEnvironment.EnvironmentName}");

                builder
                    .AddJsonFile("appsettings.json", false, false)
                    .AddJsonFile("config/appsettings.k8s.json", true, false)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, false)
                    .AddEnvironmentVariables();
            });

            StorageAreaSetting storageAreaSetting = new StorageAreaSetting();
            builder.Configuration.GetSection("ServiceSettings:StorageAreaSetting").Bind(storageAreaSetting);
            builder.Services.AddSingleton<StorageAreaSetting>(storageAreaSetting);

            builder.Services.AddDbContext<DocumentPoolAdapterContext>(options =>
            {
                options.UseOracle(builder.Configuration.GetConnectionString("docyon"));
            });

            builder.Services.AddHealthChecks().AddDbContextCheck<DocumentPoolAdapterContext>("Database", tags: new[] { "db" } , customTestQuery: (dbContext, cancellationToken) =>
            {
                using var command = dbContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SELECT 1 FROM DUAL";
                dbContext.Database.OpenConnection();
                using var reader = command.ExecuteReader();
                reader.Read();
                string result = reader.GetString(0);

                return Task.FromResult(result == "1");
            });

            // Add services to the container.
            builder.Services.AddScoped<IEntityRepository<DocumentPoolAdapter>, EntityRepository<DocumentPoolAdapter>>();
            builder.Services.AddScoped<IDiskReadAndWriteService, DiskReadAndWriteService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapHealthChecks("/actuator/health");

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}
