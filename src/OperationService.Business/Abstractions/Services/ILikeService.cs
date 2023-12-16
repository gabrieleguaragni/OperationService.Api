using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Abstractions.Services
{
    public interface ILikeService
    {
        public Task<bool> GetLikeStatus(long iduser, long idpost);
        public Task<string> ToggleLike(long iduser, long idpost);
        public Task<List<LikeResponse>> GetUserLikes(long iduser);
        public Task<List<LikeResponse>> GetPostLikes(long idpost);
    }
}
