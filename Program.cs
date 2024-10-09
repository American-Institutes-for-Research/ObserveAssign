using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ObserveAssign.Data;
using ObserveAssign.Services;

namespace ObserveAssign
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            builder.Services.AddDbContextPool<ObserveAssignDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.Configure<FormOptions>(options =>
            {
                // Set the limit to 1 GB - causes 400 error to be shown to users if not set - default in .NET is lower
                options.MultipartBodyLengthLimit = 1090519040;
                options.MemoryBufferThreshold = 1090519040;
            });

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Home");
                options.Conventions.AuthorizeFolder("/Project");
                options.Conventions.AuthorizeFolder("/School");
                options.Conventions.AuthorizeFolder("/Tool");
                options.Conventions.AuthorizeFolder("/User");
                options.Conventions.AuthorizeFolder("/UserVideo");
                options.Conventions.AuthorizeFolder("/Videos");
                options.Conventions.AllowAnonymousToPage("/Index");
                options.Conventions.AllowAnonymousToPage("/Privacy");
            });
                        
            builder.Services.AddKendo();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            var app = builder.Build();

            using (var serviceScope = app.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                SeedRolesAndAdminUser(services).Wait();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }

        /// <summary>
        /// Created using CoPilot
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var userStore = serviceProvider.GetRequiredService<IUserStore<IdentityUser>>();
            
            string[] roleNames = { "Administrator", "Video Viewer" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmailAddr = "admin@admin.com";
            var adminUser = new IdentityUser
            {
                UserName = adminEmailAddr,
                NormalizedUserName = adminEmailAddr.ToUpper(),
                Email = adminEmailAddr,
                NormalizedEmail = adminEmailAddr.ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = false,
                AccessFailedCount = 0,
            };

            // Placeholder value to start the site - will need to be updated on initial login.
            string adminPassword = "Admin@123";
            var user = await userManager.FindByEmailAsync(adminEmailAddr);

            if (user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);

                if (createAdminUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }
        }
    }
}