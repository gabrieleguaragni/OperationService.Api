using Microsoft.EntityFrameworkCore;
using OperationService.Repository.Models;

namespace OperationService.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommentsTable>().ToTable("Comments");
            modelBuilder.Entity<CommentsTable>().HasKey(x => x.IDComment);
            modelBuilder.Entity<CommentsTable>().Property(e => e.IDComment).ValueGeneratedOnAdd();

            modelBuilder.Entity<FollowersTable>().ToTable("Followers");
            modelBuilder.Entity<FollowersTable>().HasKey(x => x.IDFollower);
            modelBuilder.Entity<FollowersTable>().Property(e => e.IDFollower).ValueGeneratedOnAdd();

            modelBuilder.Entity<LikesTable>().ToTable("Likes");
            modelBuilder.Entity<LikesTable>().HasKey(x => x.IDLike);
            modelBuilder.Entity<LikesTable>().Property(e => e.IDLike).ValueGeneratedOnAdd();

            modelBuilder.Entity<PostsTable>().ToTable("Posts");
            modelBuilder.Entity<PostsTable>().HasKey(x => x.IDPost);
            modelBuilder.Entity<PostsTable>().Property(e => e.IDPost).ValueGeneratedOnAdd();

            modelBuilder.Entity<LikesTable>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.IDPost);

            modelBuilder.Entity<CommentsTable>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.IDPost);
        }

        public DbSet<CommentsTable> Comments { get; set; }
        public DbSet<FollowersTable> Followers { get; set; }
        public DbSet<LikesTable> Likes { get; set; }
        public DbSet<PostsTable> Posts { get; set; }
    }
}
