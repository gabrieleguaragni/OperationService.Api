using OperationService.Business.Abstractions.Kafka;
using OperationService.Business.Abstractions.Services;
using OperationService.Business.Exceptions;
using OperationService.Repository;
using OperationService.Repository.Models;
using OperationService.Shared.DTO.Kafka;
using OperationService.Shared.DTO.Response;

namespace OperationService.Business.Services
{
    public class FollowerService : IFollowerService
    {
        private readonly IRepository _repository;
        private readonly IKafkaProducerService _kafkaProducerService;

        public FollowerService(
            IRepository repository,
            IKafkaProducerService kafkaProducerService
            )
        {
            _repository = repository;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<bool> GetFollowStatus(long iduser, long idfollow)
        {
            if (iduser == idfollow)
            {
                throw new HttpStatusException(400, "A user cannot follow himself");
            }

            FollowersTable? follow = await _repository.GetFollow(iduser, idfollow);
            return follow != null;
        }

        public async Task<string> ToggleFollow(long iduser, long idfollow)
        {
            if (iduser == idfollow)
            {
                throw new HttpStatusException(400, "A user cannot follow himself");
            }

            FollowersTable? follow = await _repository.GetFollow(iduser, idfollow);
            if (follow == null)
            {
                FollowersTable addFollow = new()
                {
                    IDUser = iduser,
                    IDFollow = idfollow,
                    Date = DateTime.Now
                };
                await _repository.InsertFollow(addFollow);
                await _repository.SaveChangesAsync();

                await _kafkaProducerService.ProduceNotificationAsync(new SendNotificationKafka()
                {
                    IDUser = idfollow,
                    Message = "New follower",
                    Type = NotificationType.Info
                });

                return "Follow";
            }
            else
            {
                _repository.DeleteFollow(follow);
                await _repository.SaveChangesAsync();
                return "Unfollow";
            }
        }

        public async Task<List<FollowResponse>> GetUserFollows(long iduser)
        {
            List<FollowResponse> follows = new();
            foreach (var item in await _repository.GetFollows(iduser))
            {
                follows.Add(new FollowResponse()
                {
                    IDFollow = item.IDFollow,
                    Date = item.Date
                });
            }

            return follows;
        }

        public async Task<List<FollowerResponse>> GetUserFollowers(long iduser)
        {
            List<FollowerResponse> followers = new();
            foreach (var item in await _repository.GetFollowers(iduser))
            {
                followers.Add(new FollowerResponse()
                {
                    IDUser = item.IDUser,
                    Date = item.Date
                });
            }

            return followers;
        }

        public async Task<List<PostResponse>> GetUserFeed(long iduser)
        {
            List<PostResponse> feed = new();
            foreach (var item in await _repository.GetFeed(iduser))
            {
                feed.Add(new PostResponse()
                {
                    IDPost = item.IDPost,
                    IDUser = item.IDUser,
                    Image = item.Image,
                    Text = item.Text,
                    Date = item.Date
                });
            }

            return feed;
        }
    }
}
