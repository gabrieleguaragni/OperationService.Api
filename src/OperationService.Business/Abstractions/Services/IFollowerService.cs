using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Abstractions.Services
{
    public interface IFollowerService
    {
        public Task<bool> GetFollowStatus(long iduser, long idfollow);
        public Task<string> ToggleFollow(long iduser, long idfollow);
        public Task<List<FollowResponse>> GetUserFollows(long iduser);
        public Task<List<FollowerResponse>> GetUserFollowers(long iduser);
        public Task<List<PostResponse>> GetUserFeed(long iduser);
    }
}