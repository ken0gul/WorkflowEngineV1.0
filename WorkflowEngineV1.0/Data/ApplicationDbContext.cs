using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         //   modelBuilder.Entity<Workflow>()
         //.HasMany(w => w.Tasks)
         //.WithOne(t => t.Workflow)
         //.HasForeignKey(t => t.WorkflowId)
         //.OnDelete(DeleteBehavior.Cascade);

         //   modelBuilder.Entity<Workflow>()
         //       .HasMany(w => w.Connections)
         //       .WithOne(c => c.Workflow)
         //       .HasForeignKey(c => c.WorkflowId)
         //       .OnDelete(DeleteBehavior.Cascade);

         //   modelBuilder.Entity<Connection>()
         //       .HasOne(c => c.StartTask)
         //       .WithMany()
         //       .HasForeignKey(c => c.StartTaskId)
         //       .OnDelete(DeleteBehavior.Restrict);

         //   modelBuilder.Entity<Connection>()
         //       .HasOne(c => c.EndTask)
         //       .WithMany()
         //       .HasForeignKey(c => c.EndTaskId)
         //       .OnDelete(DeleteBehavior.Restrict);
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem { Id = 1, Name = "Task 1", X = 100, Y = 100 },
                new TaskItem { Id = 2, Name = "Task 2", X = 300, Y = 100 },
                new TaskItem { Id = 3, Name = "Task 3", X = 100, Y = 300 },
                new TaskItem { Id = 4, Name = "Task 4", X = 300, Y = 300 },
                new TaskItem { Id = 5, Name = "Task 5", X = 200, Y = 500 }
            );
        }
    }
}
