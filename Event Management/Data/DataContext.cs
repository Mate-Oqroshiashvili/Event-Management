using Event_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UsedPromoCode> UsedPromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- User Configurations ----
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Ensure unique emails

            modelBuilder.Entity<User>()
                .Property(u => u.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Purchases)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Participants)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .Property(p => p.Balance)
                .HasPrecision(18, 2);

            // ---- Organizer Configurations ----
            modelBuilder.Entity<Organizer>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organizer)
                .HasForeignKey<Organizer>(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organizer>()
                .HasMany(o => o.Events)
                .WithOne(e => e.Organizer)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Organizer>()
                .HasMany(o => o.Locations)
                .WithMany(l => l.Organizers);

            // ---- Event Configurations ----
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Tickets)
                .WithOne(t => t.Event)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Location)
                .WithMany(l => l.Events)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Ticket Configurations ----
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Users)
                .WithMany(u => u.Tickets)
                .UsingEntity<Dictionary<string, object>>(
                    "TicketUser",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Ticket>().WithMany().HasForeignKey("TicketId")
                );

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Purchases)
                .WithMany(p => p.Tickets)
                .UsingEntity<Dictionary<string, object>>(
                    "TicketPurchase",
                    j => j.HasOne<Purchase>().WithMany().HasForeignKey("PurchaseId"),
                    j => j.HasOne<Ticket>().WithMany().HasForeignKey("TicketId")
                );

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Participants)
                .WithOne(p => p.Ticket)
                .HasForeignKey(p => p.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);

            // ---- Participant Configurations ----
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Purchase) // A participant belongs to one purchase
                .WithMany(p => p.Participants) // A purchase can have many participants
                .HasForeignKey(p => p.PurchaseId) // Foreign key
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .Property(p => p.Attendance)
                .HasDefaultValue(false);

            modelBuilder.Entity<Participant>()
                .HasIndex(p => new { p.EventId, p.UserId, p.TicketId, p.Id }) // or p.PurchaseId if using PurchaseId
                .IsUnique(); // Ensure a user can only participate once per event and can have one ticket only once

            // ---- Purchase Configurations ----
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.PromoCode)
                .WithMany() // No navigation property in PromoCode
                .HasForeignKey(p => p.PromoCodeId)
                .OnDelete(DeleteBehavior.Restrict); // If a PromoCode is deleted, don't delete purchases

            modelBuilder.Entity<Purchase>()
                .Property(p => p.TotalAmount)
                .HasPrecision(18, 2);

            // ---- PromoCode Configurations ----

            // Unique Constraint for PromoCodeText (To prevent duplicates)
            modelBuilder.Entity<PromoCode>()
                .HasIndex(p => p.PromoCodeText)
                .IsUnique();

            modelBuilder.Entity<PromoCode>()
                .HasOne(pc => pc.Event)
                .WithMany(e => e.PromoCodes) // Add PromoCodes collection in Event model
                .HasForeignKey(pc => pc.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Delete promo codes when an event is deleted'

            // Define Relationship: UsedPromoCode ↔ PromoCode
            modelBuilder.Entity<UsedPromoCode>()
                .HasOne(upc => upc.PromoCode)
                .WithMany(pc => pc.UsedPromoCodes)
                .HasForeignKey(upc => upc.PromoCodeId)
                .OnDelete(DeleteBehavior.Cascade); // Prevent deleting PromoCode if used

            // ---- UsedPromoCode Configurations ----

            // Define Relationship: UsedPromoCode ↔ User
            modelBuilder.Entity<UsedPromoCode>()
                .HasOne(upc => upc.User)
                .WithMany(u => u.UsedPromoCodes)
                .HasForeignKey(upc => upc.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting User removes UsedPromoCodes

            // ---- Review Configurations ----
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.EventId })
                .IsUnique();  // Ensures a user can review an event only once

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of user

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade); // If event is deleted, its reviews are deleted too

            // ---- Comment Configurations ----
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of user

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Comments)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade); // If event is deleted, its comments are deleted too
        }
    }
}
