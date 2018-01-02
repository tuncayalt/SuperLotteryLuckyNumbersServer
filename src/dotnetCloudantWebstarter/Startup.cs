using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using CloudantDotNet.Services;
using CloudantDotNet.Tasks;
using System.Text.Encodings.Web;
using dotnetCloudantWebstarter.Cache;

namespace CloudantDotNet
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public JobManager jobManager;

        public Startup(IHostingEnvironment env)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("vcap-local.json", optional: true);
            Configuration = configBuilder.Build();

            string vcapServices = System.Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if (vcapServices != null)
            {
                dynamic json = JsonConvert.DeserializeObject(vcapServices);
                foreach (dynamic obj in json.Children())
                {
                    if (((string)obj.Name).ToLowerInvariant().Contains("cloudant"))
                    {
                        dynamic credentials = (((JProperty)obj).Value[0] as dynamic).credentials;
                        if (credentials != null)
                        {
                            string host = credentials.host;
                            string username = credentials.username;
                            string password = credentials.password;
                            Configuration["cloudantNoSQLDB:0:credentials:username"] = username;
                            Configuration["cloudantNoSQLDB:0:credentials:password"] = password;
                            Configuration["cloudantNoSQLDB:0:credentials:host"] = host;
                            break;
                        }
                    }
                }
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // works with VCAP_SERVICES JSON value added to vcap-local.json when running locally,
            // and works with actual VCAP_SERVICES env var based on configuration set above when running in CF
            var creds = new CloudantDotNet.Models.Creds()
            {
                username = Configuration["cloudantNoSQLDB:0:credentials:username"],
                password = Configuration["cloudantNoSQLDB:0:credentials:password"],
                host = Configuration["cloudantNoSQLDB:0:credentials:host"]
            };
            var cekilisCloudantService = new CekilisCloudantService(creds, UrlEncoder.Default);
            var couponsCloudantService = new CouponsCloudantService(creds, UrlEncoder.Default);
            var userService = new UserCloudantService(creds, UrlEncoder.Default);
            var mpService = new MilliPiyangoService();
            var firebaseService = new FirebasePushService();
            
            jobManager = new JobManager(cekilisCloudantService, couponsCloudantService, mpService, userService, firebaseService);
            CekilisCache cekilisCache = new CekilisCache(cekilisCloudantService);

            services.AddSingleton(typeof(CloudantDotNet.Models.Creds), creds);
            services.AddSingleton(typeof(JobManager), jobManager);
            services.AddSingleton(typeof(CekilisCache), cekilisCache);
            services.AddTransient<ICouponsCloudantService, CouponsCloudantService>();
            services.AddTransient<ICekilisCloudantService, CekilisCloudantService>();
            services.AddTransient<IUserCloudantService, UserCloudantService>();
            services.AddTransient<IMilliPiyangoService, MilliPiyangoService>();
            services.AddTransient<IPushService, FirebasePushService>();

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

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseConfiguration(config)
                        .UseStartup<Startup>()
                        .Build();

            host.Run();

            
        }
    }
}