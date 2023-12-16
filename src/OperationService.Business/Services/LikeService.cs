using OperationService.Business.Abstractions.Services;
using OperationService.Business.Exceptions;
using OperationService.Repository;
using OperationService.Repository.Models;
using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Services
{
    public class LikeService : ILikeService
    {
        private readonly IRepository _repository;

        public LikeService(
            IRepository repository
            )
        {
            _repository = repository;
        }

        public async Task<bool> GetLikeStatus(long iduser, long idpost)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            LikesTable? like = await _repository.GetLike(iduser, idpost);
            return like != null;
        }

        public async Task<string> ToggleLike(long iduser, long idpost)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            LikesTable? like = await _repository.GetLike(iduser, idpost);
            if (like == null)
            {
                LikesTable addLike = new()
                {
                    IDUser = iduser,
                    IDPost = idpost,
                    Date = DateTime.Now
                };
                await _repository.InsertLike(addLike);
                await _repository.SaveChangesAsync();
                return "Like added";
            }
            else
            {
                _repository.DeleteLike(like);
                await _repository.SaveChangesAsync();
                return "Like removed";
            }
        }

        public async Task<List<LikeResponse>> GetUserLikes(long iduser)
        {
            List<LikeResponse> likes = new();
            foreach (var item in await _repository.GetLikesByIDUser(iduser))
            {
                likes.Add(new LikeResponse()
                {
                    IDLike = item.IDLike,
                    IDPost = item.IDPost,
                    Date = item.Date,
                    Post = new PostResponse()
                    {
                        IDPost = item.Post.IDUser,
                        IDUser = item.Post.IDUser,
                        Image = item.Post.Image,
                        Text = item.Post.Text,
                        Date = item.Post.Date
                    }
                });
            }

            return likes;
        }

        public async Task<List<LikeResponse>> GetPostLikes(long idpost)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            List<LikeResponse> likes = new();
            foreach (var item in await _repository.GetLikesByIDPost(idpost))
            {
                likes.Add(new LikeResponse()
                {
                    IDLike = item.IDLike,
                    IDUser = item.IDUser,
                    Date = item.Date
                });
            }

            return likes;
        }
    }
}
