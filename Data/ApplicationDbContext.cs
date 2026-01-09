using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using WorkTrack.App.Models;

namespace WorkTrack.App.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =======================
        // DbSet Properties
        // =======================
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<TasksItem> Tasks { get; set; } = null!;
        public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
        public DbSet<TaskHistory> TaskHistories { get; set; } = null!;
        public DbSet<Timeline> Timelines { get; set; } = null!;
        public DbSet<ManagerTeam> ManagerTeams { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =======================
            // Project
            // =======================
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.ProjectCode)
                .IsUnique();

            modelBuilder.Entity<TasksItem>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TasksItem>()
                .HasOne(t => t.AssignedBy)
                .WithMany()
                .HasForeignKey(t => t.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // ApplicationUser self-reference (Manager)
            // =======================
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Manager)
                .WithMany(u => u.Subordinates)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // ProjectMember
            // =======================
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany()
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany()
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // TaskHistory
            // =======================
            modelBuilder.Entity<TaskHistory>()
                .HasOne(th => th.Task)
                .WithMany()
                .HasForeignKey(th => th.TasksId) // Corrected FK property name
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskHistory>()
                .HasOne(th => th.ChangedBy)
                .WithMany()
                .HasForeignKey(th => th.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // Timeline
            // =======================
            modelBuilder.Entity<Timeline>()
                .HasOne(tl => tl.Project)
                .WithMany()
                .HasForeignKey(tl => tl.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths

            // =======================
            // ManagerTeam
            // =======================
            modelBuilder.Entity<ManagerTeam>()
                .HasOne(mt => mt.Manager)
                .WithMany()
                .HasForeignKey(mt => mt.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManagerTeam>()
                .HasOne(mt => mt.Employee)
                .WithMany()
                .HasForeignKey(mt => mt.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
