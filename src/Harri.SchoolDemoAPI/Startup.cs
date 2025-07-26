using System;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Harri.SchoolDemoAPI.Filters;
using Harri.SchoolDemoAPI.OpenApi;
using Harri.SchoolDemoAPI.Repository;
using HealthChecks.UI.Client;
using Microsoft.Net.Http.Headers;

namespace Harri.SchoolDemoAPI
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesBase(services, null);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="healthChecksOverride">Action to set health checks instead of default checks</param>
        public void ConfigureServicesBase(IServiceCollection services, Action<IHealthChecksBuilder>? healthChecksOverride)
        {
            // Add framework services.
            services
                // Don't need the full MVC stack for an API, see https://andrewlock.net/comparing-startup-between-the-asp-net-core-3-templates/
                .AddControllers(options =>
                {
                    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            
            services.AddCors(options => {
                options.AddDefaultPolicy(
                        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                            .WithExposedHeaders(HeaderNames.ETag); }
                    );
            });

            var sqlServerConnectionString = Configuration["SQLConnectionString"];
            var healthChecksBuilder = services.AddHealthChecks();
            if (healthChecksOverride is null)
            {
                healthChecksBuilder.AddSqlServer(sqlServerConnectionString, name: "sql", timeout: TimeSpan.FromSeconds(10));
            }
            else
            {
                healthChecksOverride.Invoke(healthChecksBuilder);
            }
            
            services
                .AddSwaggerGen(c =>
                {
                    c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                    
                    c.SwaggerDoc("1.0.0", new OpenApiInfo
                    {
                        Title = "Swagger School ADMIN - OpenAPI 3.0",
                        Description = "Swagger School ADMIN - OpenAPI 3.0 (ASP.NET Core 8.0)",
                        TermsOfService = new Uri("https://github.com/openapitools/openapi-generator"),
                        Contact = new OpenApiContact
                        {
                            Name = "OpenAPI-Generator Contributors",
                            Url = new Uri("https://github.com/openapitools/openapi-generator"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "NoLicense",
                            Url = new Uri("http://localhost")
                        },
                        Version = "1.0.0",
                    });
                    c.CustomSchemaIds(type => type.FriendlyId(true));
                    //c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");

                    // Include DataAnnotation attributes on Controller Action parameters as OpenAPI validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });

            services.AddApplicationInsightsTelemetry(options => {
                options.EnableRequestTrackingTelemetryModule = false; // Requests are tracked with request and response body with .AddHttpLogging below
            });

            services.AddHttpLogging(options =>
            {
                // LoggingFields includes the request and response body,
                // in a production scenario you probably want to exclude body and only log santised data
                options.LoggingFields = HttpLoggingFields.RequestQuery
                    | HttpLoggingFields.RequestMethod
                    | HttpLoggingFields.RequestPath
                    | HttpLoggingFields.RequestBody
                    | HttpLoggingFields.ResponseStatusCode
                    | HttpLoggingFields.ResponseBody
                    | HttpLoggingFields.Duration;
                options.MediaTypeOptions.Clear();
                options.MediaTypeOptions.AddText("application/json"); // Don't log the response body for swagger
                options.CombineLogs = true;
            });

            // Dependency Injection
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(sqlServerConnectionString));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger(c =>
                {
                    c.RouteTemplate = "openapi/{documentName}/openapi.json";
                })
                .UseSwaggerUI(c =>
                {
                    // set route prefix to openapi, e.g. http://localhost:8080/openapi/index.html
                    c.RoutePrefix = "openapi";

                    c.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger School ADMIN - OpenAPI 3.0");
                });

            app.UseHttpLogging();

            app.UseRouting();

            app.UseCors();
            
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                });
        }
    }
}
