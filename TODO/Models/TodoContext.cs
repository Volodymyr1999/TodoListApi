using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.Models
{
    /*  Database*/
    public class TodoContext:DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options):base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public DbSet<CustomList> customLists { get; set; }
        public DbSet<Importance> Importances { get; set; }
       
        public DbSet<Task> Tasks { get; set; }
        
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<Importance>().HasIndex(p => p.DigitValue).IsUnique();
            modelBuilder.Entity<Importance>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<User>().HasIndex(p => p.UserName).IsUnique();
            modelBuilder.Entity<Task>().HasOne(p => p.User).WithMany(p => p.Tasks).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);

            
        }
    }
}
