using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;

namespace DataAccess.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Project>()
            .HasOne(p => p.ProjectStatus)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.TaskStatus)
            .WithMany(s => s.TaskItems)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.TaskProject)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.TaskUser)
                .WithMany()                  
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
