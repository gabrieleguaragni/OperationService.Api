using Microsoft.EntityFrameworkCore.Storage;
using OperationService.Repository.Models;

namespace OperationService.Repository
{
    public interface IRepository
    {
        public void SaveChanges();
        public Task SaveChangesAsync();
        public IDbContextTransaction BeginTransaction();
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task InsertPost(PostsTable post);
        public Task<PostsTable?> GetPostById(long idpost);
        public Task<List<PostsTable>> GetPostsByIDUser(long iduser);
        public void DeletePost(PostsTable post);
        public Task<LikesTable?> GetLike(long iduser, long idpost);
        public Task InsertLike(LikesTable like);
        public void DeleteLike(LikesTable like);
        public Task<List<LikesTable>> GetLikesByIDUser(long iduser);
        public Task<List<LikesTable>> GetLikesByIDPost(long idpost);
        public Task<CommentsTable?> GetCommentById(long idcomment);
        public Task<CommentsTable?> GetComment(long iduser, long idpost);
        public Task InsertComment(CommentsTable comment);
        public void DeleteComment(CommentsTable comment);
        public Task<List<CommentsTable>> GetCommentsByIDUser(long idpost);
        public Task<List<CommentsTable>> GetCommentsByIDPost(long idpost);
        public Task InsertFollow(FollowersTable follow);
        public void DeleteFollow(FollowersTable follow);
        public Task<FollowersTable?> GetFollow(long iduser, long idfollow);
        public Task<List<FollowersTable>> GetFollows(long iduser);
        public Task<List<FollowersTable>> GetFollowers(long iduser);
        public Task<List<PostsTable>> GetFeed(long iduser);
    }
}
