using Microsoft.EntityFrameworkCore;
using TestAssignment.Models;

namespace TestAssignment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserTask> Tasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Uniqueness for Username and Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Automatically set creation and update time
            modelBuilder.Entity<User>()
                 .Property(u => u.CreatedAt)
                 .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .IsRequired();

            modelBuilder.Entity<UserTask>()
                .Property(t => t.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<UserTask>()
                .Property(t => t.UpdatedAt)
                .IsRequired();

            // One to many relationship between User and UserTask
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(ut => ut.UserId);
        }
    }
}
