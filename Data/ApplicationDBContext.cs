using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WEB_2023.Config;
using WEB_2023.Entities;


namespace WEB_2023.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // SeedData(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            var Roles = new
            {
                admin = new IdentityRole
                {
                    Name = AppRoles.Admin,
                    NormalizedName = AppRoles.Admin.ToUpper()
                },
                customer = new IdentityRole
                {
                    Name = AppRoles.Customer,
                    NormalizedName = AppRoles.Customer.ToUpper()
                }
            };
            var Users = new
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    FullName = "Nguyễn Ngọc Anh Tuấn",
                    Avatar = "/uploads/images/admin.webp",
                    Address = "Phường Bến Thuỷ, TP Vinh, Nghệ An",
                    CreatedAt = DateTime.UtcNow,
                    DateOfBirth = new DateOnly(2002, 07, 02),
                    PhoneNumber = "0123456789"
                },
                customer = new ApplicationUser
                {
                    UserName = "customer",
                    NormalizedUserName = "CUSTOMER",
                    Email = "customer@customer.com",
                    NormalizedEmail = "CUSTOMER@CUSTOMER.COM",
                    EmailConfirmed = true,
                    FullName = "Phan Thị Huyền",
                    Avatar = "/img/default-user.webp",
                    Address = "Phường Bến Thuỷ, TP Vinh, Nghệ An",
                    CreatedAt = DateTime.UtcNow,
                    DateOfBirth = new DateOnly(2002, 09, 02),
                    PhoneNumber = "0123456789"
                }
            };
            var UserRoles = new
            {
                admin = new IdentityUserRole<string>
                {
                    RoleId = Roles.admin.Id,
                    UserId = Users.admin.Id
                },
                customer = new IdentityUserRole<string>
                {
                    RoleId = Roles.customer.Id,
                    UserId = Users.customer.Id
                }
            };
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            Users.admin.PasswordHash = passwordHasher.HashPassword(Users.admin, "Admin@123");
            Users.customer.PasswordHash = passwordHasher.HashPassword(Users.customer, "Customer@123");
            modelBuilder.Entity<ApplicationUser>().HasData(Users.admin, Users.customer);
            modelBuilder.Entity<IdentityRole>().HasData(Roles.admin, Roles.customer);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(UserRoles.admin, UserRoles.customer);
        }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<BlogComment> BlogComments { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Shipping> Shippings { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
    }
}