using AspNetCoreIdentityExamples.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentityExamples.Models;
using AspNetCoreIdentityExamples.CustomValidations;
using Microsoft.AspNetCore.Http;
using System;
using EmailService;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityExamples.CustomTokenProviders;
namespace AspNetCoreIdentityExamples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); ;

            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration.GetConnectionString("Default"));
                opts.LogTo(Console.WriteLine);
                opts.EnableSensitiveDataLogging();
            });

            services.AddIdentity<AppUser, AppRole>(opts =>
            {
                opts.Password.RequiredLength = 8;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;

                
                opts.Lockout.AllowedForNewUsers = true;
                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opts.Lockout.MaxFailedAccessAttempts = 3;

                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcçdefghiıjklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._@+";

                opts.SignIn.RequireConfirmedEmail= true;

                opts.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            })
            .AddPasswordValidator<CustomPasswordValidator>()
            .AddUserValidator<CustomUserValidation>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddEntityFrameworkStores<AppDbContext>()
             .AddDefaultTokenProviders().
             AddTokenProvider<EmailConfirmationTokenProvider<AppUser>>("emailconfirmation"); ;

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromMinutes(1));

            services.Configure<EmailConfirmationTokenProviderOptions>(opts =>
            opts.TokenLifespan = TimeSpan.FromDays(7)
            );


            // Cookie authu təmin etməyin bir yolu budur.
            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = "/Account/Login";
                opts.LogoutPath = "/Account/Logout";
                opts.Cookie = new CookieBuilder
                {
                    Name = "AspNetCoreIdentityExamplesCookie", //Cookienin adı
                    //Cookienin brovserdəki ömrü
                    HttpOnly = true,// Brovser cookieləri sadəcə http mesajları yollayarkən əlçatan olmalıdır, JS kodu ilə əldə edilə bilməməlidir. Əgər false qoysaq, JS kodu ilə də(client side kodu ilə yəni) əldə edilməsinə icazə vermiş olarıq
                    SameSite = SameSiteMode.Lax,
                    //Fərqli bir domaindən, yəni üçüncü tərəfdən istək gəldikdə cookieləri göndərib-göndərməməklə bağlıdır.
                    //None - 3cü tərəfdən gələn hər istəyə yolla, amma secure contextdə. Yəni Secure HTTPS olsa.
                    //Strict - 3-cü tərəfdən gələn heç bir istəyə yollama,
                    //Lax - Əgər request GEt-dirsə, və url-də dəyişikliyə,(top level navigation)-a səbəb olursa cookieləri göndər

                    //3-cü tərəf istəklərə cross-site istəklər də deyilir, yəni bir link varsa, səni A saytından B-saytına aparırsa, belə istəklər cross-sitedır

                    SecurePolicy = CookieSecurePolicy.Always
                    // Always - Cookielər ancaq HTTPS ilə əlçatandır.
                    // SameAsRequest - Həm HTTPS, həm HTTP
                    // None - HTTP

                
                };
                opts.ExpireTimeSpan = TimeSpan.FromDays(7);
                opts.SlidingExpiration = false;
                
                
            });

            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
