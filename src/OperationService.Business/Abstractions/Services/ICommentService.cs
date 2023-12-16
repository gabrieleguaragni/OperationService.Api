using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Abstractions.Services
{
    public interface ICommentService
    {
        public Task<CommentResponse> GetComment(long idcomment);
        public Task<CommentResponse> InsertComment(long iduser, long idpost, string text);
        public Task<CommentResponse> UpdateComment(long idRequestingUser, List<string?> roles, long idcomment, string text);
        public Task DeleteComment(long idRequestingUser, List<string?> roles, long idcomment);
        public Task<List<CommentResponse>> GetUserComments(long iduser);
        public Task<List<CommentResponse>> GetPostComments(long idpost);
    }
}
