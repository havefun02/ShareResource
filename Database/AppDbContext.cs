using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShareResource.Models.Entities;
namespace ShareResource.Database
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Img> Imgs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ImgTags> ImgTags { get; set; }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
        }
        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseTime>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);


                entity.Property(u => u.UserId)
                    .IsRequired();

                entity.Property(u => u.UserName)
                    .HasMaxLength(50)
                    .IsRequired(); 

                entity.Property(u => u.UserEmail)
                    .HasMaxLength(100) 
                    .IsRequired(); 

                entity.Property(u => u.UserPhone)
                    .HasMaxLength(15);

                entity.HasOne(u => u.UserRole)
                    .WithMany(r=>r.RoleUsers) 
                    .HasForeignKey(u => u.UserRoleId) // Foreign key for UserRole
                    .OnDelete(DeleteBehavior.Restrict); // Configure delete behavior
                entity.HasOne(u => u.UserToken)
                    .WithOne(t=>t.User)
                    .HasForeignKey<Token>(t=>t.UserId) 
                    .OnDelete(DeleteBehavior.Cascade); // Configure delete behavior
                entity.HasOne(u => u.UserToken).WithOne(t => t.User).HasForeignKey<Token>(t => t.UserId);
                //var user = new User
                //{
                //    UserId=Guid.NewGuid().ToString(),
                //    UserEmail = "Admin@gmail.com",
                //    UserName = "Lapphan",
                //    UserPhone = "123456789",
                //    UserRoleId = "Admin",
                //};
                //var ownerUser = new User
                //{
                //    UserId = Guid.NewGuid().ToString(),
                //    UserEmail = "Owner@gmail.com",
                //    UserName = "Lapphan",
                //    UserPhone = "123456789",
                //    UserRoleId = "Owner",
                //};
                //user.UserPassword = new PasswordHasher<User>().HashPassword(user,"adminpassword");
                //ownerUser.UserPassword = new PasswordHasher<User>().HashPassword(ownerUser, "adminpassword");

                //entity.HasData([user, ownerUser]);
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);

                entity.Property(r => r.RoleId)
                      .IsRequired();

                entity.Property(r => r.RoleName)
                      .HasMaxLength(50) 
                      .IsRequired();
                entity.HasIndex(r => r.RoleName)
                    .IsUnique();

                entity.Property(r => r.RoleDescription)
                      .HasMaxLength(200);
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(p => p.PermissionId);
                entity.Property(p => p.PermissionName).HasMaxLength(50).IsRequired();
            });
            modelBuilder.Entity<RolePermission>(entity => {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                entity.HasOne(rp=>rp.Role).WithMany(r=>r.RolePermissions).HasForeignKey(rp=>rp.RoleId);
                entity.HasOne(rp => rp.Permission).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.PermissionId);
            });
            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(t=> t.TokenId);
                entity.Property(t=>t.IsRevoked).HasDefaultValue(false);
            });

            modelBuilder.Entity<ImgLovers>(entity => {
                entity.HasKey(t => new { t.UserId, t.ImgId });
                entity.HasOne(t=>t.User).WithMany(u=>u.ImgLovers).HasForeignKey(u=>u.ImgId);
                entity.HasOne(t => t.Img).WithMany(i=>i.ImgLovers).HasForeignKey(i=>i.ImgId);
            });

            modelBuilder.Entity<Img>(entity =>
            {
                entity.HasKey(e => e.ImgId);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255); 

                entity.Property(e => e.FileUrl)
                    .IsRequired();  

                entity.Property(e => e.ContentType)
                    .IsRequired()  
                    .HasMaxLength(50);  

                entity.Property(e => e.FileSize)
                    .IsRequired() 
                    .HasDefaultValue(0) 
                    .HasColumnType("bigint");

                entity.Property(e => e.UploadDate)
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .IsRequired(); 

                entity.HasOne(e => e.User)  
                    .WithMany(u => u.UserImgs)  
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade); 

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.IsPrivate)
                    .IsRequired(); 
            });
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.TagId);
                entity.Property(e => e.TagName).IsRequired().HasMaxLength(100);
            });
            modelBuilder.Entity<ImgTags>(entity => {
                entity.HasKey(it => new { it.TagId, it.ImgId });
                entity.HasOne(it=>it.Imgs).WithMany(i=>i.ImgTags).HasForeignKey(it => it.ImgId);
                entity.HasOne(it => it.Tags).WithMany(i => i.ImgTags).HasForeignKey(it => it.TagId);

            });
            //var permisisons = new List<Permission>();
            //permisisons.Add(new Permission { PermissionId = "Read",PermissionName="Read" });
            //permisisons.Add(new Permission { PermissionId = "Write", PermissionName = "Write" });
            //permisisons.Add(new Permission { PermissionId = "Delete", PermissionName = "Delete" });
            //permisisons.Add(new Permission { PermissionId = "Execute", PermissionName = "Execute" });
            //permisisons.Add(new Permission { PermissionId = "FullPermissions", PermissionName = "FullPermissions" });
            //modelBuilder.Entity<Permission>().HasData(permisisons);

            //var roles = new List<Role>();
            //roles.Add(new Role { RoleId = "Admin", RoleName = "Admin" });
            //roles.Add(new Role { RoleId = "Owner", RoleName = "Owner" });
            //roles.Add(new Role { RoleId = "Guest", RoleName = "Guest" });
            //roles.Add(new Role { RoleId = "User", RoleName = "User" });

            //modelBuilder.Entity<Role>().HasData(roles);
            
            //var rolePermissions = new List<RolePermission>();
            //rolePermissions.Add(new RolePermission { RoleId = "Admin", PermissionId = "Read" });
            //rolePermissions.Add(new RolePermission { RoleId = "Admin", PermissionId = "Write" });
            //rolePermissions.Add(new RolePermission { RoleId = "Admin", PermissionId = "Delete" });

            //rolePermissions.Add(new RolePermission { RoleId = "Guest", PermissionId = "Read" });
            //rolePermissions.Add(new RolePermission { RoleId = "User", PermissionId = "Read" });
            //rolePermissions.Add(new RolePermission { RoleId = "User", PermissionId = "Write" });
            //rolePermissions.Add(new RolePermission { RoleId = "Owner", PermissionId = "FullPermissions" });
            //modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                var configSection = configBuilder.GetSection("ConnectionStrings");
                var connectionString = configSection["DefaultConnection"];
                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
            }
        }
    }
}
