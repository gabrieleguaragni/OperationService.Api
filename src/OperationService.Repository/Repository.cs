using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OperationService.Repository.Models;
using System.Data;

namespace OperationService.Repository
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public Repository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void SaveChanges() => _applicationDbContext.SaveChanges();

        public async Task SaveChangesAsync() => await _applicationDbContext.SaveChangesAsync();

        public IDbContextTransaction BeginTransaction() => _applicationDbContext.Database.BeginTransaction();

        public async Task<IDbContextTransaction> BeginTransactionAsync() => await _applicationDbContext.Database.BeginTransactionAsync();

        public async Task InsertPost(PostsTable post)
        {
            await _applicationDbContext.Posts.AddAsync(post);
        }

        public async Task<PostsTable?> GetPostById(long idpost)
        {
            return await _applicationDbContext.Posts.Where(x => x.IDPost == idpost).Include(x => x.Likes).Include(x => x.Comments).SingleOrDefaultAsync();
        }

        public async Task<List<PostsTable>> GetPostsByIDUser(long iduser)
        {
            return await _applicationDbContext.Posts.Where(x => x.IDUser == iduser).Include(x => x.Likes).Include(x => x.Comments).ToListAsync();
        }

        public void DeletePost(PostsTable post)
        {
            _applicationDbContext.Posts.Remove(post);
        }

        public async Task<LikesTable?> GetLike(long iduser, long idpost)
        {
            return await _applicationDbContext.Likes.Where(x => x.IDUser == iduser && x.IDPost == idpost).Include(x => x.Post).SingleOrDefaultAsync();
        }

        public async Task InsertLike(LikesTable like)
        {
            await _applicationDbContext.Likes.AddAsync(like);
        }

        public void DeleteLike(LikesTable like)
        {
            _applicationDbContext.Likes.Remove(like);
        }

        public async Task<List<LikesTable>> GetLikesByIDUser(long iduser)
        {
            return await _applicationDbContext.Likes.Where(x => x.IDUser == iduser).Include(x => x.Post).ToListAsync();
        }

        public async Task<List<LikesTable>> GetLikesByIDPost(long idpost)
        {
            return await _applicationDbContext.Likes.Where(x => x.IDPost == idpost).Include(x => x.Post).ToListAsync();
        }

        public async Task<CommentsTable?> GetCommentById(long idcomment)
        {
            return await _applicationDbContext.Comments.Where(x => x.IDComment == idcomment).Include(x => x.Post).SingleOrDefaultAsync();
        }

        public async Task<CommentsTable?> GetComment(long iduser, long idpost)
        {
            return await _applicationDbContext.Comments.Where(x => x.IDUser == iduser && x.IDPost == idpost).Include(x => x.Post).SingleOrDefaultAsync();
        }

        public async Task InsertComment(CommentsTable comment)
        {
            await _applicationDbContext.Comments.AddAsync(comment);
        }

        public void DeleteComment(CommentsTable comment)
        {
            _applicationDbContext.Comments.Remove(comment);
        }

        public async Task<List<CommentsTable>> GetCommentsByIDUser(long iduser)
        {
            return await _applicationDbContext.Comments.Where(x => x.IDUser == iduser).Include(x => x.Post).ToListAsync();
        }

        public async Task<List<CommentsTable>> GetCommentsByIDPost(long idpost)
        {
            return await _applicationDbContext.Comments.Where(x => x.IDPost == idpost).Include(x => x.Post).ToListAsync();
        }

        public async Task InsertFollow(FollowersTable follow)
        {
            await _applicationDbContext.Followers.AddAsync(follow);
        }

        public void DeleteFollow(FollowersTable follow)
        {
            _applicationDbContext.Followers.Remove(follow);
        }

        public async Task<FollowersTable?> GetFollow(long iduser, long idfollow)
        {
            return await _applicationDbContext.Followers.Where(x => x.IDUser == iduser && x.IDFollow == idfollow).SingleOrDefaultAsync();
        }

        public async Task<List<FollowersTable>> GetFollows(long iduser)
        {
            return await _applicationDbContext.Followers.Where(x => x.IDUser == iduser).ToListAsync();
        }

        public async Task<List<FollowersTable>> GetFollowers(long iduser)
        {
            return await _applicationDbContext.Followers.Where(x => x.IDFollow == iduser).ToListAsync();
        }

        public async Task<List<PostsTable>> GetFeed(long iduser)
        {
            return await _applicationDbContext.Followers.Where(x => x.IDUser == iduser).Join(
                _applicationDbContext.Posts,
                follower => follower.IDFollow,
                post => post.IDUser,
                (follower, post) => new { Follower = follower, Post = post }
            )
                .OrderByDescending(result => result.Post.Date)
                .Select(result => result.Post).ToListAsync();
        }
    }
}