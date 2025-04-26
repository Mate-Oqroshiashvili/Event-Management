using Event_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Event> Events { get; set; } // Collection of events
        public DbSet<Participant> Participants { get; set; } // Collection of participants
        public DbSet<Purchase> Purchases { get; set; } // Collection of purchases
        public DbSet<Ticket> Tickets { get; set; } // Collection of tickets
        public DbSet<Location> Locations { get; set; } // Collection of locations
        public DbSet<Organizer> Organizers { get; set; } // Collection of organizers
        public DbSet<User> Users { get; set; } // Collection of users
        public DbSet<PromoCode> PromoCodes { get; set; } // Collection of promo codes
        public DbSet<Review> Reviews { get; set; } // Collection of reviews
        public DbSet<Comment> Comments { get; set; } // Collection of comments
        public DbSet<UsedPromoCode> UsedPromoCodes { get; set; } // Collection of used promo codes

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Method to configure the model
        {
            // Call the base method to ensure any configurations in the base class are applied
            base.OnModelCreating(modelBuilder);

            // ---- User Configurations ----
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email) // Index for faster lookups
                .IsUnique(); // Ensure unique emails

            modelBuilder.Entity<User>()
                .Property(u => u.RowVersion) // RowVersion for concurrency control
                .IsRowVersion(); // Concurrency token for optimistic concurrency control

            modelBuilder.Entity<User>()
                .HasMany(u => u.Purchases) // A user can have many purchases
                .WithOne(p => p.User) // Each purchase belongs to one user
                .HasForeignKey(p => p.UserId) // Foreign key in Purchase table
                .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, their purchases are deleted too

            modelBuilder.Entity<User>() 
                .HasMany(u => u.Participants) // A user can have many participants
                .WithOne(p => p.User) // Each participant belongs to one user
                .HasForeignKey(p => p.UserId) // Foreign key in Participant table
                .OnDelete(DeleteBehavior.NoAction); // If a user is deleted, their participants are not deleted

            modelBuilder.Entity<User>()
                .Property(p => p.Balance) // Balance property for user
                .HasPrecision(18, 2); // Precision for monetary values

            // ---- Organizer Configurations ----
            modelBuilder.Entity<Organizer>()
                .HasOne(o => o.User) // Each organizer has one user
                .WithOne(u => u.Organizer) // Each user can have one organizer
                .HasForeignKey<Organizer>(o => o.UserId) // Foreign key in Organizer table
                .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, their organizer is deleted too

            modelBuilder.Entity<Organizer>()
                .HasMany(o => o.Events) // An organizer can have many events
                .WithOne(e => e.Organizer) // Each event belongs to one organizer
                .HasForeignKey(e => e.OrganizerId) // Foreign key in Event table
                .OnDelete(DeleteBehavior.Restrict); // If an organizer is deleted, their events are not deleted

            modelBuilder.Entity<Organizer>()
                .HasMany(o => o.Locations) // An organizer can have many locations
                .WithMany(l => l.Organizers); // Many-to-many relationship with Locations

            // ---- Event Configurations ----
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Tickets) // An event can have many tickets
                .WithOne(t => t.Event) // Each ticket belongs to one event
                .HasForeignKey(t => t.EventId) // Foreign key in Ticket table
                .OnDelete(DeleteBehavior.Cascade); // If an event is deleted, its tickets are deleted too

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Location) // Each event has one location
                .WithMany(l => l.Events) // Each location can have many events
                .HasForeignKey(e => e.LocationId) // Foreign key in Event table
                .OnDelete(DeleteBehavior.Restrict); // If a location is deleted, its events are not deleted

            // ---- Ticket Configurations ----
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Users) // A ticket can be associated with many users
                .WithMany(u => u.Tickets) // A user can have many tickets
                .UsingEntity<Dictionary<string, object>>(
                    "TicketUser",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"), // Foreign key in TicketUser table
                    j => j.HasOne<Ticket>().WithMany().HasForeignKey("TicketId") // Foreign key in TicketUser table
                ); // Many-to-many relationship with Users

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event) // Each ticket belongs to one event
                .WithMany(e => e.Tickets) // Each event can have many tickets
                .HasForeignKey(t => t.EventId) // Foreign key in Ticket table
                .OnDelete(DeleteBehavior.Cascade); // If an event is deleted, its tickets are deleted too

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Purchases) // A ticket can be associated with many purchases
                .WithMany(p => p.Tickets) // A purchase can have many tickets
                .UsingEntity<Dictionary<string, object>>(
                    "TicketPurchase",
                    j => j.HasOne<Purchase>().WithMany().HasForeignKey("PurchaseId"), // Foreign key in TicketPurchase table
                    j => j.HasOne<Ticket>().WithMany().HasForeignKey("TicketId") // Foreign key in TicketPurchase table
                ); // Many-to-many relationship with Purchases

            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Participants) // A ticket can be associated with many participants
                .WithOne(p => p.Ticket) // Each participant belongs to one ticket
                .HasForeignKey(p => p.TicketId) // Foreign key in Participant table
                .OnDelete(DeleteBehavior.Restrict); // If a ticket is deleted, its participants are not deleted

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Price) // Price property for ticket
                .HasPrecision(18, 2); // Precision for monetary values

            // ---- Participant Configurations ----
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Event) // A participant belongs to one event
                .WithMany(e => e.Participants) // An event can have many participants
                .HasForeignKey(p => p.EventId) // Foreign key in Participant table
                .OnDelete(DeleteBehavior.Cascade); // If an event is deleted, its participants are deleted too

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Purchase) // A participant belongs to one purchase
                .WithMany(p => p.Participants) // A purchase can have many participants
                .HasForeignKey(p => p.PurchaseId) // Foreign key
                .OnDelete(DeleteBehavior.Cascade); // If a purchase is deleted, its participants are deleted too

            modelBuilder.Entity<Participant>()
                .Property(p => p.Attendance) // Attendance property for participant
                .HasDefaultValue(false); // Default value for attendance

            modelBuilder.Entity<Participant>()
                .HasIndex(p => new { p.EventId, p.UserId, p.TicketId, p.Id }) // or p.PurchaseId if using PurchaseId
                .IsUnique(); // Ensure a user can only participate once per event and can have one ticket only once

            // ---- Purchase Configurations ----
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User) // A purchase belongs to one user
                .WithMany(u => u.Purchases) // A user can have many purchases
                .HasForeignKey(p => p.UserId) // Foreign key in Purchase table
                .OnDelete(DeleteBehavior.Restrict); // If a user is deleted, their purchases are not deleted

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.PromoCode) // A purchase can have one promo code
                .WithMany() // No navigation property in PromoCode
                .HasForeignKey(p => p.PromoCodeId) // Foreign key in Purchase table
                .OnDelete(DeleteBehavior.Restrict); // If a PromoCode is deleted, don't delete purchases

            modelBuilder.Entity<Purchase>()
                .Property(p => p.TotalAmount) // TotalAmount property for purchase
                .HasPrecision(18, 2); // Precision for monetary values

            // ---- PromoCode Configurations ----

            // Unique Constraint for PromoCodeText (To prevent duplicates)
            modelBuilder.Entity<PromoCode>()
                .HasIndex(p => p.PromoCodeText) // Index for faster lookups
                .IsUnique(); // Ensure unique promo codes

            modelBuilder.Entity<PromoCode>()
                .HasOne(pc => pc.Event) // Each promo code belongs to one event
                .WithMany(e => e.PromoCodes) // Add PromoCodes collection in Event model
                .HasForeignKey(pc => pc.EventId) // Foreign key in PromoCode table
                .OnDelete(DeleteBehavior.Cascade); // Delete promo codes when an event is deleted'

            // Define Relationship: UsedPromoCode ↔ PromoCode
            modelBuilder.Entity<UsedPromoCode>()
                .HasOne(upc => upc.PromoCode) // Each used promo code belongs to one promo code
                .WithMany(pc => pc.UsedPromoCodes) // Each promo code can have many used promo codes
                .HasForeignKey(upc => upc.PromoCodeId) // Foreign key in UsedPromoCode table
                .OnDelete(DeleteBehavior.Cascade); // Prevent deleting PromoCode if used

            // ---- UsedPromoCode Configurations ----

            // Define Relationship: UsedPromoCode ↔ User
            modelBuilder.Entity<UsedPromoCode>()
                .HasOne(upc => upc.User) // Each used promo code belongs to one user
                .WithMany(u => u.UsedPromoCodes) // Each user can have many used promo codes
                .HasForeignKey(upc => upc.UserId) // Foreign key in UsedPromoCode table
                .OnDelete(DeleteBehavior.Cascade); // Deleting User removes UsedPromoCodes

            // ---- Review Configurations ----
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.EventId }) // Composite index for UserId and EventId
                .IsUnique();  // Ensures a user can review an event only once

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User) // Each review belongs to one user
                .WithMany(u => u.Reviews) // A user can have many reviews
                .HasForeignKey(r => r.UserId) // Foreign key in Review table
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of user

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Event) // Each review belongs to one event
                .WithMany(e => e.Reviews) // An event can have many reviews
                .HasForeignKey(r => r.EventId) // Foreign key in Review table
                .OnDelete(DeleteBehavior.Cascade); // If event is deleted, its reviews are deleted too

            // ---- Comment Configurations ----
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User) // Each comment belongs to one user
                .WithMany(u => u.Comments) // A user can have many comments
                .HasForeignKey(c => c.UserId) // Foreign key in Comment table
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of user

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Event) // Each comment belongs to one event
                .WithMany(e => e.Comments) // An event can have many comments
                .HasForeignKey(c => c.EventId) // Foreign key in Comment table
                .OnDelete(DeleteBehavior.Cascade); // If event is deleted, its comments are deleted too
        }
    }
}
