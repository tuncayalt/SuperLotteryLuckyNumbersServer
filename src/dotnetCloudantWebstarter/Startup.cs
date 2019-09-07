using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CloudantDotNet.Services;
using CloudantDotNet.Tasks;
using System.Text.Encodings.Web;
using dotnetCloudantWebstarter.Cache;
using dotnetCloudantWebstarter.Services;
using CloudantDotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;
using System.IO;

namespace CloudantDotNet
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        public JobManager jobManager;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddJsonFile("vcap-local.json", optional: true) // when running locally, store VCAP_SERVICES credentials in vcap-local.json
            .AddEnvironmentVariables();

            Configuration = builder.Build();

            string vcapServices = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if (vcapServices != null)
            {
                dynamic json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(vcapServices);

                // CF 'cloudantNoSQLDB' service
                if (json.ContainsKey("cloudantNoSQLDB"))
                {
                    try
                    {
                        Configuration["cloudantNoSQLDB:0:credentials:username"] = json["cloudantNoSQLDB"][0].credentials.username;
                        Console.WriteLine("username ");
                        Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:username"]);
                        Configuration["cloudantNoSQLDB:0:credentials:password"] = json["cloudantNoSQLDB"][0].credentials.password;
                        Console.WriteLine("password ");
                        Console.WriteLine(json["cloudantNoSQLDB"][0].credentials.password);
                        Configuration["cloudantNoSQLDB:0:credentials:host"] = json["cloudantNoSQLDB"][0].credentials.host;
                        Console.WriteLine("host ");
                        Console.WriteLine(json["cloudantNoSQLDB"][0].credentials.host);
                        Configuration["cloudantNoSQLDB:0:credentials:url"] = json["cloudantNoSQLDB"][0].credentials.url;
                        Console.WriteLine("url ");
                        Console.WriteLine(json["cloudantNoSQLDB"][0].credentials.url);
                    }
                    catch (Exception)
                    {
                        // Failed to read Cloudant uri, ignore this and continue without a database
                    }
                }
                // user-provided service with 'cloudant' in the name
                else if (json.ContainsKey("user-provided"))
                {
                    foreach (var service in json["user-provided"])
                    {
                        if (((string)service.name).Contains("cloudant"))
                        {
                            try
                            {
                                Configuration["cloudantNoSQLDB:0:credentials:username"] = json["cloudantNoSQLDB"][0].credentials.username;
                                Configuration["cloudantNoSQLDB:0:credentials:password"] = json["cloudantNoSQLDB"][0].credentials.password;
                                Configuration["cloudantNoSQLDB:0:credentials:host"] = json["cloudantNoSQLDB"][0].credentials.host;
                                Configuration["cloudantNoSQLDB:0:credentials:url"] = json["cloudantNoSQLDB"][0].credentials.url;
                            }
                            catch (Exception)
                            {
                                // Failed to read Cloudant uri, ignore this and continue without a database
                            }
                        }
                    }
                }

            }
            else if (Configuration.GetSection("services").Exists())
            {
                try
                {
                    Configuration["cloudantNoSQLDB:0:credentials:username"] = Configuration["services:cloudantNoSQLDB:0:credentials:username"];
                    Console.WriteLine("username ");
                    Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:username"]);
                    Configuration["cloudantNoSQLDB:0:credentials:password"] = Configuration["services:cloudantNoSQLDB:0:credentials:password"];
                    Console.WriteLine("password ");
                    Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:password"]);
                    Configuration["cloudantNoSQLDB:0:credentials:host"] = Configuration["services:cloudantNoSQLDB:0:credentials:host"];
                    Console.WriteLine("host ");
                    Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:host"]);
                    Configuration["cloudantNoSQLDB:0:credentials:url"] = Configuration["services:cloudantNoSQLDB:0:credentials:url"];
                    Console.WriteLine("url ");
                    Console.WriteLine(Configuration["cloudantNoSQLDB:0:credentials:url"]);
                }
                catch (Exception)
                {
                    // Failed to read Cloudant uri, ignore this and continue without a database
                }

            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // works with VCAP_SERVICES JSON value added to vcap-local.json when running locally,
            // and works with actual VCAP_SERVICES env var based on configuration set above when running in CF
            var creds = new Creds()
            {
                username = Configuration["cloudantNoSQLDB:0:credentials:username"],
                password = Configuration["cloudantNoSQLDB:0:credentials:password"],
                host = Configuration["cloudantNoSQLDB:0:credentials:host"]
            };

            if (creds.username != null && creds.password != null && creds.host != null)
            {
                services.AddAuthorization();
                services.AddSingleton(typeof(Creds), creds);
                

                services.AddTransient<LoggingHandler>();
                services.AddHttpClient("cloudant", client =>
                {
                    if (creds.username == null || creds.password == null || creds.host == null)
                    {
                        throw new Exception("Missing Cloudant NoSQL DB service credentials");
                    }

                    var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(creds.username + ":" + creds.password));

                    client.BaseAddress = new Uri(Configuration["cloudantNoSQLDB:0:credentials:url"]);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
                })
                .AddHttpMessageHandler<LoggingHandler>();

                //var cekilisCloudantService = new CekilisCloudantService(creds, UrlEncoder.Default, );
                //var couponsCloudantService = new CouponsCloudantService(creds, UrlEncoder.Default);
                //var userService = new UserCloudantService(creds, UrlEncoder.Default);
                //var configCloudantService = new ConfigCloudantService(creds, UrlEncoder.Default);
                //var mpService = new MilliPiyangoService();
                //var firebaseService = new FirebasePushService();

                

                services.AddSingleton(typeof(Creds), creds);
                
                services.AddSingleton(typeof(CekilisCache));
                services.AddSingleton(typeof(ConfigCache));
                services.AddTransient<ICouponsCloudantService, CouponsCloudantService>();
                services.AddTransient<ICekilisCloudantService, CekilisCloudantService>();
                services.AddTransient<IConfigCloudantService, ConfigCloudantService>();
                services.AddTransient<IUserCloudantService, UserCloudantService>();
                services.AddTransient<IMilliPiyangoService, MilliPiyangoService>();
                services.AddTransient<IPushService, FirebasePushService>();
                //jobManager = new JobManager(cekilisCloudantService, couponsCloudantService, mpService, userService, firebaseService);
                
                services.AddSingleton(typeof(JobManager));

                // Build an intermediate service provider
                var sp = services.BuildServiceProvider();

                // Resolve the services from the service provider
                var cekilisCache = sp.GetService<CekilisCache>();
                var configCache = sp.GetService<ConfigCache>();
            }

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Populate test data in the database
            //var cloudantService = ((ICloudantService)app.ApplicationServices.GetService(typeof(ICloudantService)));
            //cloudantService.PopulateTestData().Wait();

            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        
    }
}