using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using AutoMapper;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using InventoryAPI.Infrastructure;
using InventoryAPI.Infrastructure.Filters;
using InventoryAPI.IntegrationEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InventoryAPI
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

      builder.AddEnvironmentVariables();

      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Add framework services.
      services.AddMvc(options =>
      {
        options.Filters.Add(typeof(HttpGlobalExceptionFilter));
      }).AddControllersAsServices();

      services.AddAutoMapper(typeof(Startup));

      services.AddDbContext<InventoryContext>(options =>
      {
        options.UseSqlServer(Configuration["ConnectionString"],
          sqlOptions =>
          {
            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
          });
        options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
      });

      services.Configure<Settings>(Configuration);

      services.AddSwaggerGen();
      services.ConfigureSwaggerGen(options =>
      {
        options.DescribeAllEnumsAsStrings();
        options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info
        {
          Title = "Warehouse - Inventory API",
          Version = "v1",
          Description = "The Inventory Microservice HTTP API",
          TermsOfService = "Terms Of Service"
        });
      });

      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
          builder => builder.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials());
      });

      services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
        sp => (DbConnection c) => new IntegrationEventLogService(c));
      var serviceProvider = services.BuildServiceProvider();
      var configuration = serviceProvider.GetRequiredService<IOptionsSnapshot<Settings>>().Value;
      services.AddTransient<IInventoryIntegrationEventService, InventoryIntegrationEventService>();
      services.AddSingleton<IEventBus>(new EventBusRabbitMQ.EventBusRabbitMQ(configuration.EventBusConnection));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseCors("CorsPolicy");

      app.UseMvcWithDefaultRoute();

      app.UseSwagger()
        .UseSwaggerUi();

      var context = (InventoryContext) app.ApplicationServices.GetService(typeof(InventoryContext));

      WaitForSqlAvailability(context, loggerFactory);

      InventoryContextSeed.SeedAsync(app, loggerFactory)
        .Wait();

      var integrationEventLogContext = new IntegrationEventLogContext(
        new DbContextOptionsBuilder<IntegrationEventLogContext>()
        .UseSqlServer(Configuration["ConnectionString"], b => b.MigrationsAssembly("InventoryAPI"))
        .Options);
      try
      {
        integrationEventLogContext.Database.Migrate();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void WaitForSqlAvailability(InventoryContext context, ILoggerFactory loggerFactory, int? retry = 0)
    {
      int retryForAvailability = retry.Value;
      try
      {
        context.Database.OpenConnection();
      }
      catch (SqlException e)
      {
        if (retryForAvailability < 10)
        {
          retryForAvailability++;
          var log = loggerFactory.CreateLogger(nameof(Startup));
          log.LogError(e.Message);
          WaitForSqlAvailability(context, loggerFactory, retryForAvailability);
        }
      }
      finally
      {
        context.Database.CloseConnection();
      }
    }
  }
}
