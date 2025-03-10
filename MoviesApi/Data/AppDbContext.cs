using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;

namespace MoviesApi.Data
{
    /// <summary>
    /// Represents the application's database context.
    /// Manages the connection to the database and defines the entity relationships.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AppDbContext"/>.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the Users table in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the FavoriteMovies table in the database.
        /// </summary>
        public DbSet<FavoriteMovie> FavoriteMovies { get; set; }

        /// <summary>
        /// Configures the entity relationships and database constraints.
        /// </summary>
        /// <param name="modelBuilder">Model builder instance for configuring the database schema.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationship: A FavoriteMovie belongs to a User, with cascading delete.
            modelBuilder.Entity<FavoriteMovie>()
                .HasOne(f => f.User)
                .WithMany(u => u.FavoriteMovies)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure the Email field in the Users table is unique.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Ensure that a user cannot have duplicate favorite movies by title.
            modelBuilder.Entity<FavoriteMovie>()
                .HasIndex(f => new { f.UserId, f.Title })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
