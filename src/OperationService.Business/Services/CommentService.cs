using OperationService.Business.Abstractions.Kafka;
using OperationService.Business.Abstractions.Services;
using OperationService.Business.Exceptions;
using OperationService.Repository;
using OperationService.Repository.Models;
using OperationService.Shared.DTO.Kafka;
using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Services
{
    public class CommentService : ICommentService
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IRepository _repository;
        private readonly string administrator = "administrator";
        private readonly string developer = "developer";
        private readonly string moderator = "moderator";

        public CommentService(
            IKafkaProducerService kafkaProducerService,
            IRepository repository
            )
        {
            _repository = repository;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<CommentResponse> GetComment(long idcomment)
        {
            CommentsTable? comment = await _repository.GetCommentById(idcomment);
            if (comment == null)
            {
                throw new HttpStatusException(404, "This comment not exists");
            }

            return new()
            {
                IDComment = comment.IDComment,
                IDUser = comment.IDUser,
                IDPost = comment.IDPost,
                Text = comment.Text,
                Date = comment.Date
            };
        }

        public async Task<CommentResponse> InsertComment(long iduser, long idpost, string text)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new HttpStatusException(400, "Empty comment not allowed");
            }

            CommentsTable addComment = new()
            {
                IDUser = iduser,
                IDPost = idpost,
                Text = text,
                Date = DateTime.Now
            };
            await _repository.InsertComment(addComment);
            await _repository.SaveChangesAsync();

            return new()
            {
                IDComment = addComment.IDComment,
                IDUser = addComment.IDUser,
                IDPost = addComment.IDPost,
                Text = addComment.Text,
                Date = addComment.Date
            };
        }

        public async Task<CommentResponse> UpdateComment(long idRequestingUser, List<string?> roles, long idcomment, string text)
        {
            CommentsTable? comment = await _repository.GetCommentById(idcomment);
            if (comment == null)
            {
                throw new HttpStatusException(404, "This comment not exists");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new HttpStatusException(400, "Empty comment not allowed");
            }

            // if the user requesting the update is different from the owner of the comment
            if (idRequestingUser != comment.IDUser)
            {
                roles = roles.Where(x => x != null).ToList();
                if (!roles.Contains(administrator) && !roles.Contains(developer) && !roles.Contains(moderator))
                {
                    throw new HttpStatusException(403, "Invalid permission to update another user's comment");
                }
            }

            if (comment.Text == text)
            {
                throw new HttpStatusException(400, "Cannot be assigned the same value as Text");
            }

            comment.Text = text;
            comment.Date = DateTime.Now;
            await _repository.SaveChangesAsync();

            return new()
            {
                IDComment = comment.IDComment,
                IDUser = comment.IDUser,
                IDPost = comment.IDPost,
                Text = comment.Text,
                Date = comment.Date,
            };
        }

        public async Task DeleteComment(long idRequestingUser, List<string?> roles, long idcomment)
        {
            CommentsTable? comment = await _repository.GetCommentById(idcomment);
            if (comment == null)
            {
                throw new HttpStatusException(404, "This comment not exists");
            }

            // if the user requesting deletion is different from the owner of the comment
            if (idRequestingUser != comment.IDUser)
            {
                roles = roles.Where(x => x != null).ToList();
                if (!roles.Contains(administrator) && !roles.Contains(developer) && !roles.Contains(moderator))
                {
                    throw new HttpStatusException(403, "Invalid permission to delete another user's comment");
                }

                if (roles.Contains(moderator))
                {
                    await _kafkaProducerService.ProduceNotificationAsync(new SendNotificationKafka()
                    {
                        IDUser = comment.IDUser,
                        Message = "Comment deleted because it doesn't respect our policies",
                        Type = NotificationType.Warning
                    });
                }
            }

            _repository.DeleteComment(comment);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<CommentResponse>> GetUserComments(long iduser)
        {
            List<CommentResponse> comments = new();
            foreach (var item in await _repository.GetCommentsByIDUser(iduser))
            {
                comments.Add(new CommentResponse()
                {
                    IDComment = item.IDComment,
                    IDPost = item.IDPost,
                    Text = item.Text,
                    Date = item.Date
                });
            }

            return comments;
        }

        public async Task<List<CommentResponse>> GetPostComments(long idpost)
        {
            PostsTable? post = await _repository.GetPostById(idpost);
            if (post == null)
            {
                throw new HttpStatusException(404, "This post not exists");
            }

            List<CommentResponse> comments = new();
            foreach (var item in await _repository.GetCommentsByIDPost(idpost))
            {
                comments.Add(new CommentResponse()
                {
                    IDComment = item.IDComment,
                    IDUser = item.IDUser,
                    Text = item.Text,
                    Date = item.Date
                });
            }

            return comments;
        }
    }
}
