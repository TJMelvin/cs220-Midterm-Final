using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection.Emit;
using TylerMelvin_DiscussionBoard.Models;
using TylerMelvin_DiscussionBoard.Repos;

namespace TylerMelvin_DiscussionBoard.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _config;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        //db sets
        public DbSet<DiscussionThread> DiscussionThreads { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _config != null)
            {
                var conn = _config.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(conn))
                {
                    optionsBuilder.UseSqlServer(conn);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            //identity models
            base.OnModelCreating(modelbuilder);

            //config timestamps
            modelbuilder.Entity<DiscussionThread>()
                .Property(e => e.Timestamp)
                .IsConcurrencyToken();

            modelbuilder.Entity<Post>()
                .Property(e => e.Timestamp)
                .IsConcurrencyToken();

            modelbuilder.Entity<DiscussionThread>()
                .HasIndex(x => x.CreatedAt)
                .IsUnique();

            modelbuilder.Entity<Post>()
                .HasIndex(x => new { x.ApplicationUserId, x.CreatedAt })
                .IsUnique();

            modelbuilder.Entity<Post>()
                .HasOne(p => p.ParentPost)
                .WithMany()
                .HasForeignKey(p => p.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelbuilder.Entity<DiscussionThread>()
                .HasMany(dt => dt.Posts)
                .WithOne(p => p.DiscussionThread)
                .HasForeignKey(p => p.DiscussionThreadId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Property("Timestamp").CurrentValue = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    bool identicalValues = true;

                    foreach (var property in proposedValues.Properties)
                    {
                        var proposedValue = proposedValues[property];
                        var databaseValue = databaseValues[property];

                        if (!object.Equals(proposedValue, databaseValue))
                        {
                            identicalValues = false;
                            break;
                        }
                    }
                    if (identicalValues)
                    {
                        return base.SaveChanges();
                    }
                    entry.OriginalValues.SetValues(databaseValues);
                }
                throw;
            }
        }
    }
}
