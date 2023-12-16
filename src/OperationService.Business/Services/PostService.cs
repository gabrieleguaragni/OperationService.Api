using Microsoft.AspNetCore.Http;
using OperationService.Business.Abstractions.Kafka;
using OperationService.Business.Abstractions.Services;
using OperationService.Business.Exceptions;
using OperationService.Repository;
using OperationService.Repository.Models;
using OperationService.Shared.DTO.Kafka;
using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Services
{
    public class PostService : IPostService
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IRepository _repository;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;
        private readonly string administrator = "administrator";
        private readonly string developer = "developer";
        private readonly string moderator = "moderator";

        public PostService(
            IKafkaProducerService kafkaProducerService,
            IRepository repository, 
            ILikeService likeService,
            ICommentService commentService
            )
        {
            _kafkaProducerService = kafkaProducerService;
            _repository = repository;
            _likeService = likeService;
            _commentService = commentService;
        }

        public async Task<PostResponse> CreatePost(long iduser, IFormFile post, string text)
        {
            string uniqueFileName = $"{iduser}_{DateTime.Now.Ticks}_{Guid.NewGuid()}";
            using var memoryStream = new MemoryStream();
            await post.CopyToAsync(memoryStream);
            await _kafkaProducerService.ProducePostAsync(new SendPostKafka()
            {
                IDUser = iduser,
                File = Convert.ToBase64String(memoryStream.ToArray()),
                FileExtension = Path.GetExtension(post.FileName),
                FileName = uniqueFileName
            });

            PostsTable newPost = new()
            {
                IDUser = iduser,
                Image = $"{uniqueFileName}.png",
                Text = text,
                Date = DateTime.Now
            };

            await _repository.InsertPost(newPost);
            await _repository.SaveChangesAsync();

            return new() {
                IDPost = newPost.IDPost,
                IDUser = newPost.IDUser,
                Image = newPost.Image,
                Text = newPost.Text,
                Date = newPost.Date
            };
        }

        public async Task<PostResponse> UpdatePost(long idRequestingUser, List<string?> roles, long idpost, string text)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            // if the user requesting the update is different from the owner of the post
            if (idRequestingUser != post.IDUser)
            {
                roles = roles.Where(x => x != null).ToList();
                if (!roles.Contains(administrator) && !roles.Contains(developer) && !roles.Contains(moderator))
                {
                    throw new HttpStatusException(403, "Invalid permission to update another user's post");
                }
            }

            post.Text = text;
            await _repository.SaveChangesAsync();
            return new()
            {
                IDPost = post.IDPost,
                IDUser = post.IDUser,
                Image = post.Image,
                Text = post.Text,
                Date = post.Date
            };
        }

        public async Task DeletePost(long idRequestingUser, List<string?> roles, long idpost)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            // if the user requesting deletion is different from the owner of the post
            if (idRequestingUser != post.IDUser) 
            {
                roles = roles.Where(x => x != null).ToList();
                if (!roles.Contains(administrator) && !roles.Contains(developer) && !roles.Contains(moderator))
                {
                    throw new HttpStatusException(403, "Invalid permission to delete another user's post");
                }

                if (roles.Contains(moderator))
                {
                    await _kafkaProducerService.ProduceNotificationAsync(new SendNotificationKafka()
                    {
                        IDUser = post.IDUser,
                        Message = "Post deleted because it doesn't respect our policies",
                        Type = NotificationType.Warning
                    });
                }
            }

            _repository.DeletePost(post);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<PostResponse>> GetPosts(long iduser)
        {
            List<PostResponse> posts = new();
            foreach (var item in await _repository.GetPostsByIDUser(iduser))
            {
                posts.Add(new PostResponse()
                {
                    IDPost = item.IDPost,
                    IDUser = item.IDUser,
                    Image = item.Image,
                    Text = item.Text,
                    Date = item.Date
                });
            }

            return posts;
        }

        public async Task<PostResponse> GetPost(long idpost)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            return new()
            {
                IDPost = post.IDPost,
                IDUser = post.IDUser,
                Image = post.Image,
                Text = post.Text,
                Date = post.Date,
                Likes = await _likeService.GetPostLikes(idpost),
                Comments = await _commentService.GetPostComments(idpost)
            };
        }
    }
}