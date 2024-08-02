using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Models;

namespace WorkflowEngineV1._0.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Document> Documents { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                    modelBuilder.Entity<Workflow>()
               .HasKey(w => w.Id);

            modelBuilder.Entity<Document>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Workflow)
                .WithMany() // Adjust if Workflow has navigation property for Documents
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.NoAction); // Use Restrict instead of Cascade

            // Ensure that Workflow does not have conflicting cascade paths
            modelBuilder.Entity<Workflow>()
                .HasOne(w => w.Document) // Assuming a Document navigation property in Workflow
                .WithMany()
                .HasForeignKey(w => w.DocumentId)
                .OnDelete(DeleteBehavior.NoAction); // Use Restrict instead of Cascade

             modelBuilder.Entity<Workflow>()
               .HasOne(w => w.Document)
               .WithOne(d => d.Workflow)
               .HasForeignKey<Document>(d => d.WorkflowId)
               .OnDelete(DeleteBehavior.Cascade);
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TaskItem>().HasData(
            //    new TaskItem { Id = 1, Name = "Task 1", X = 100, Y = 100 },
            //    new TaskItem { Id = 2, Name = "Task 2", X = 300, Y = 100 },
            //    new TaskItem { Id = 3, Name = "Task 3", X = 100, Y = 300 },
            //    new TaskItem { Id = 4, Name = "Task 4", X = 300, Y = 300 },
            //    new TaskItem { Id = 5, Name = "Task 5", X = 200, Y = 500 }
            //);
        }
    }
}
