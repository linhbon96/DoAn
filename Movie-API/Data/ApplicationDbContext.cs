using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Models;

namespace MovieBookingApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ItemOrder> ItemOrders { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Showtime> ShowTimes { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TicketInfo> TicketInfos { get; set; }
    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ItemOrder>()
                .HasKey(io => new { io.OrderId, io.ItemId });
            // Movie Configuration
            modelBuilder.Entity<Movie>()
                .Property(m => m.Description)
                .HasColumnType("text");

            // Item Configuration
            modelBuilder.Entity<Item>()
                .Property(i => i.Name)
                .HasColumnType("varchar(255)");

            // ItemOrder Configuration
            modelBuilder.Entity<ItemOrder>()
                .Property(io => io.Quantity)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ItemOrder>()
                .HasOne(io => io.Item)
                .WithMany()
                .HasForeignKey(io => io.ItemId);

            modelBuilder.Entity<ItemOrder>()
                .HasOne(io => io.Order)
                .WithMany(o => o.ItemOrders)
                .HasForeignKey(io => io.OrderId);

            // User Configuration
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasColumnType("varchar(255)")
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .HasColumnType("varchar(255)")
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasColumnType("varchar(50)")
                .IsRequired();

            // Seat Configuration
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.ShowTime)
                .WithMany(st => st.Seats)
                .HasForeignKey(s => s.ShowTimeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seat>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            // Order Configuration
            modelBuilder.Entity<Order>()
                .HasMany(o => o.ItemOrders)
                .WithOne(io => io.Order)
                .HasForeignKey(io => io.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ticket Configuration
            modelBuilder.Entity<Ticket>()
                .HasKey(t => t.TicketId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany()
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Theater)
                .WithMany()
                .HasForeignKey(t => t.TheaterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Showtime)
                .WithMany()
                .HasForeignKey(t => t.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);
            

            // TicketInfo Configuration
            modelBuilder.Entity<TicketInfo>()
                .HasOne(ti => ti.Ticket)
                .WithMany(t => t.TicketInfos)
                .HasForeignKey(ti => ti.TicketId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TicketInfo>()
                .HasOne(ti => ti.Order)
                .WithMany(o => o.TicketInfos)
                .HasForeignKey(ti => ti.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TicketInfo>()
                .HasOne(ti => ti.User)
                .WithMany(u => u.TicketInfos)
                .HasForeignKey(ti => ti.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho Showtime
            modelBuilder.Entity<Showtime>()
                .HasOne(st => st.Movie)
                .WithMany(m => m.Showtimes) // Một Movie có nhiều Showtime
                .HasForeignKey(st => st.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Showtime>()
                .HasOne(st => st.Theater)
                .WithMany(t => t.Showtimes) // Một Theater có nhiều Showtime
                .HasForeignKey(st => st.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Nếu Showtime có Seats
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.ShowTime)
                .WithMany(st => st.Seats) // Một Showtime có nhiều Seat
                .HasForeignKey(s => s.ShowTimeId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Theater>()
                .HasMany(t => t.Showtimes)
                .WithOne(s => s.Theater)
                .HasForeignKey(s => s.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}