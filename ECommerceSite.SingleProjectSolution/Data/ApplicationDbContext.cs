using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECommerceSite.SingleProjectSolution.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceSite.SingleProjectSolution.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRoles,int>
    {
        public ApplicationDbContext() : base()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            try
            {
                Database.Migrate();
            }
            catch(Exception)
            {

            }

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // EnableRetryOnFailure adds default SqlServerRetryingExecutionStrategy
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=SampleECommerceWebsite.SingleProjectSolution;Trusted_Connection=True;MultipleActiveResultSets=true;");
            }
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer(
            //@"Server=(localdb)\mssqllocaldb;Database=AaronCottrillSpyStore;Trusted_Connection=True;MultipleActiveResultSets=true;",
            //options => options.ExecutionStrategy(c=>new MyExecutionStrategy(c)));
            //}
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer(
            //@"Server=(localdb)\mssqllocaldb;Database=AaronCottrillSpyStore;Trusted_Connection=True;MultipleActiveResultSets=true;",
            //options => options.EnableRetryOnFailure());
            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.Email).HasName("IX_Customers").IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ShipDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.OrderTotal)
                    .HasColumnType("money")
                    //.HasComputedColumnSql("Store.GetOrderTotal([Id])")
                    ;
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.LineItemTotal)
                    .HasColumnType("money")
                    .HasComputedColumnSql("[Quantity]*[UnitCost]");

                entity.Property(e => e.UnitCost).HasColumnType("money");
            });


            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.UnitCost).HasColumnType("money");
                entity.Property(e => e.CurrentPrice).HasColumnType("money");
            });

            modelBuilder.Entity<ShoppingCartRecord>(entity =>
            {
                entity.HasIndex(e => new { ShoppingCartRecordId = e.Id, e.ProductId, e.CustomerId })
                .HasName("IX_ShoppingCart").IsUnique();

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Quantity)
                    .ValueGeneratedNever()
                    .HasDefaultValue(1);
            });

        }

        public static async Task CreateAdminAccount(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<ApplicationRoles> roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRoles>>();

            string username = configuration["UserData:AdminUser:Name"];
            string email = configuration["UserData:AdminUser:Email"];
            string password = configuration["UserData:AdminUser:Password"];
            string role = configuration["UserData:AdminUser:Role"];

            if (await userManager.FindByNameAsync(username) == null)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new ApplicationRoles(role));
                }
                ApplicationUser user = new ApplicationUser
                {
                    UserName = username,
                    Email = email
                };
                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    foreach(IdentityError error in result.Errors)
                    {
                        Console.Error.WriteLine(error.ToString());
                    }
                    
                    //throw new Exception(result.Errors.Last().ToString());
                }
            }
        }

        public DbSet<Product> Product { get; set; }
    }
}
