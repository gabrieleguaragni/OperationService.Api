using Microsoft.AspNetCore.Http;
using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Abstractions.Services
{
    public interface IPostService
    {
        public Task<PostResponse> CreatePost(long iduser, IFormFile post, string text);
        public Task<PostResponse> UpdatePost(long idRequestingUser, List<string?> roles, long idpost, string text);
        public Task DeletePost(long idRequestingUser, List<string?> roles, long idpost);
        public Task<List<PostResponse>> GetPosts(long iduser);
        public Task<PostResponse> GetPost(long idpost);
    }
}
