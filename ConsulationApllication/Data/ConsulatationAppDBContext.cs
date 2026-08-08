using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ConsulationApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsulationApplication.Data
{
    public class ConsulatationAppDBContext : IdentityDbContext<AppUser>
    {
        public ConsulatationAppDBContext(DbContextOptions<ConsulatationAppDBContext> options) : base(options)
        {
        }

        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<ConsultantService> ConsultantServices { get; set; }

        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bookings relationships
            modelBuilder.Entity<Bookings>()
                .HasOne(b => b.Client)
                .WithMany(u => u.ClientBookings)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bookings>()
                .HasOne(b => b.Consultant)
                .WithMany(u => u.ConsultantBookings)
                .HasForeignKey(b => b.ConsultantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bookings>()
                .HasOne(b => b.Service)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceId);

            // Slot relationships
            modelBuilder.Entity<Bookings>()
                .HasOne(b => b.Slot)
                .WithOne(s => s.Booking)
                .HasForeignKey<Bookings>(b => b.SlotId);

            modelBuilder.Entity<Slot>()
                .HasOne(s => s.Consultant)
                .WithMany(u => u.Slots) // consultant owns many slots
                .HasForeignKey(s => s.ConsultantId);

            // Consultant ↔ Service many-to-many
            modelBuilder.Entity<ConsultantService>()
                .HasKey(cs => new { cs.ConsultantId, cs.ServiceId });

            modelBuilder.Entity<ConsultantService>()
                .HasOne(cs => cs.Consultant)
                .WithMany(c => c.ConsultantServices)
                .HasForeignKey(cs => cs.ConsultantId);

            modelBuilder.Entity<ConsultantService>()
                .HasOne(cs => cs.Service)
                .WithMany(s => s.ConsultantServices)
                .HasForeignKey(cs => cs.ServiceId);

            modelBuilder.Entity<Services>()
                .HasOne(d => d.Department)//one department
                .WithMany(d => d.Services)// many services
                .HasForeignKey(d => d.DepartmentId)//foreign key in services table
                .OnDelete(DeleteBehavior.Cascade);



        }
    }
}