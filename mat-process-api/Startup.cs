using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mat_process_api.UseCase.V1;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Infrastructure;
using mat_process_api.V1.UseCase;
using mat_process_api.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using mat_process_api.V1.Validators;

namespace mat_process_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static List<ApiVersionDescription> _apiVersions { get; set; }
        //TODO update the below to the name of your API
        private const string ApiName = "mat-process";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(option =>
            {
                option.AddPolicy("AllowAny", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();
            ConfigureApiVersioningAndSwagger(services);
            ConfigureDbContext(services);
            RegisterGateWays(services);
            RegisterUseCases(services);
            RegisterValidators(services);
        }

        private static void ConfigureApiVersioningAndSwagger(IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1,0);
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });

            services.AddSwaggerGen(c =>
            {
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    { "Token", Enumerable.Empty<string>() }
                };

                c.AddSecurityDefinition("Token",
                  new ApiKeyScheme
                  {
                      In = "header",
                      Description = "Your Hackney API Key",
                      Name = "X-Api-Key",
                      Type = "apiKey"
                  }
                );

                c.AddSecurityRequirement(security);

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var versions = apiDesc.ControllerAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    var any = versions.Any(v => $"{v.GetFormattedApiVersion()}" == docName);
                    return any;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in _apiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion.ToString()}";
                    c.SwaggerDoc(version, new Info
                    {
                        Title = $"mat-process-api {version}",
                        Version = version,
                        Description = "This is the Hackney Property API which allows client applications to securely retrieve property information for a given property reference."
                    });
                }
                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            services.Configure<ConnectionSettings>(options =>
            {
                options.ConnectionString
                    = Environment.GetEnvironmentVariable("DocumentDbConnString");
                options.Database
                    = Environment.GetEnvironmentVariable("DatabaseName");
                options.CollectionName
                    = Environment.GetEnvironmentVariable("CollectionName");
            });

            services.AddSingleton<IMatDbContext,MatDbContext>();
        }

        private static void RegisterGateWays(IServiceCollection services)
        {
            services.AddSingleton<IProcessDataGateway, ProcessDataGateway>();

        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddSingleton<IProcessData, ProcessDataUseCase>();
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            services.AddSingleton<IPostInitialProcessDocumentRequestValidator, PostInitialProcessDocumentRequestValidator>();
            services.AddSingleton<IUpdateProcessDocumentRequestValidator, UpdateProcessDocumentRequestValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            //Get All ApiVersions,
            _apiVersions = api.ApiVersionDescriptions.Select(s => s).ToList();
            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in _apiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"mat-process-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });

            app.UseSwagger();
            app.UseCors(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });

            app.UseMvc(routes =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
