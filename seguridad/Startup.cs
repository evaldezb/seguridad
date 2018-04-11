using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using seguridad.Data;
using seguridad.Models;
using seguridad.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Azure.KeyVault;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace seguridad
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //configurationBuilder = configurationBuilder;


        }

        public IConfiguration Configuration { get; }
        public IConfigurationBuilder configurationBuilder { get; set; }

        // This method gets call0ed by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddDistributedRedisCache(o => {
                o.Configuration = "gpdemoredist.westus2.cloudapp.azure.com,password=foobared,defaultDatabase=0";
            });


            services.AddMemoryCache();

            services.AddSession(s=>s.IdleTimeout = TimeSpan.FromDays(30));

            //configurationBuilder.AddAzureKeyVault(
            //          $"https://{Configuration["Vault"]}.vault.azure.net/",
            //          Configuration["ClientId"],
            //          Configuration["ClientSecret"]);



            services.AddResponseCaching();
            services.AddResponseCompression();


            services.AddDataProtection()                
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"C:\Dataprotec"))
                .ProtectKeysWithDpapi()            
                //.ProtectKeysWithCertificate(new System.Security.Cryptography.X509Certificates.X509Certificate2(@"C:\Users\grego\Desktop\certificado test.pfx","260689"))
                .UseCustomCryptographicAlgorithms(new CngCbcAuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = "AES",
                    EncryptionAlgorithmKeySize = 256
                });

                










            var basePolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().Build();

            // services.AddAuthorization(a => a.DefaultPolicy = basePolicy);

            services.AddAuthorization(s =>
               s.AddPolicy("AdminPolicy", d => d.RequireClaim(ClaimTypes.Role, "Admin").Build()));


          

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc(
                a => a.Filters.Add(new AuthorizeFilter(basePolicy)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseResponseCaching();
            app.UseResponseCompression();

            app.UseSession();


            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
