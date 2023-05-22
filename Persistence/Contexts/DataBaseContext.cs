using Application.Interface.Contexts;
using Domain.Attribute;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence.Contexts
{
    public class DataBaseContext :DbContext,IDataBaseContext
    {
        public DataBaseContext(DbContextOptions options) :base(options)
        {

        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().Property<DateTime?>("InsertTime");
            //modelBuilder.Entity<User>().Property<DateTime?>("UpdateTime");
            foreach (var item in modelBuilder.Model.GetEntityTypes())
            {
                if (item.ClrType.GetCustomAttributes(typeof(AuditableAttribute),true).Length > 0)
                {
                    modelBuilder.Entity(item.Name).Property<DateTime>("InsertTime");
                    modelBuilder.Entity(item.Name).Property<DateTime?>("UpdateTime");
                    modelBuilder.Entity(item.Name).Property<DateTime?>("RemovedTime");
                    modelBuilder.Entity(item.Name).Property<bool?>("IsRemoved");
                }
            }
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(p=>p.State == EntityState.Modified ||
                 p.State == EntityState.Added || p.State == EntityState.Deleted);
            foreach (var item in modifiedEntries)
            {
                var entityType = item.Context.Model.FindEntityType(item.Entity.GetType());
                var inserted = entityType.FindProperty("InsertTime");
                var update = entityType.FindProperty("UpdateTime");
                var remove = entityType.FindProperty("RemoveTime");
                var Isremove = entityType.FindProperty("IsRemove");

                if(item.State == EntityState.Added && inserted != null) 
                {
                    item.Property("InserteTime").CurrentValue = DateTime.Now;
                }
                if (item.State == EntityState.Modified && update != null)
                {
                    item.Property("UpdateTIme").CurrentValue = DateTime.Now;
                }
                if (item.State == EntityState.Deleted && remove != null && Isremove != null)
                {
                    item.Property("RemoveTime").CurrentValue = DateTime.Now;
                    item.Property("IsRemove").CurrentValue = true;
                }
            }
            return base.SaveChanges();
        }
    }
}
