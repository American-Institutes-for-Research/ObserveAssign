using Microsoft.EntityFrameworkCore;
using ObserveAssign.Models;

namespace ObserveAssign.Data
{
    public class ObserveAssignDbContext : DbContext
    {
        public ObserveAssignDbContext(DbContextOptions<ObserveAssignDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRoleModel>()
                .HasKey(u => new { u.UserId, u.RoleId });
            modelBuilder.Entity<UserProjectModel>()
                .HasKey(u => new { u.AspNetUserID, u.ProjectId });
        }
        public DbSet<LogModel> LogModel { get; set; }
        public DbSet<ProjectModel> ProjectModel { get; set; }
        public DbSet<RoleModel> RoleModel { get; set; }
        public DbSet<SchoolModel> SchoolModel { get; set; }
        public DbSet<TeacherModel> TeacherModel { get; set; }
        public DbSet<ToolModel> ToolModel { get; set; }
        public DbSet<AspNetUserModel> AspNetUserModel { get; set; }
        public DbSet<UserProjectModel> UserProjectModel { get; set; }
        public DbSet<UserRoleModel> UserRoleModel { get; set; }
        public DbSet<UserVideoModel> UserVideoModel { get; set; }
        public DbSet<VideoModel> VideoModel { get; set; }
    }
}