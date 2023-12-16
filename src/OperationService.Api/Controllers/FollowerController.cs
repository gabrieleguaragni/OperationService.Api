using Microsoft.AspNetCore.Mvc;
using OperationService.Business.Abstractions.Services;
using OperationService.Shared.DTO.Request;
using OperationService.Shared.DTO.Response;

namespace OperationService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FollowerController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IFollowerService _followerService;

        public FollowerController(
            IAuthService authService,
            IFollowerService followerService
            )
        {
            _authService = authService;
            _followerService = followerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFollowStatus([FromBody] FollowRequest followRequest) // Check if a user follows another user
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            bool status = await _followerService.GetFollowStatus(followRequest.IDUser, followRequest.IDFollow);
            return Ok(new { message = status ? "Follow" : "Not follow", status });
        }

        [HttpPost("toggle/{IDFollow}")]
        public async Task<IActionResult> ToggleFollow([FromRoute] long IDFollow) // Add or remove follow
        {
            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            string result = await _followerService.ToggleFollow(validateTokenResponse.IDUser, IDFollow);
            return Ok(new { message = result });
        }

        [HttpGet("follows/{IDUser}")]
        public async Task<IActionResult> GetUserFollows([FromRoute] long IDUser) // Get follows of a user
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<FollowResponse> follows = await _followerService.GetUserFollows(IDUser);
            return Ok(new { IDUser, follows, count = follows.Count });
        }

        [HttpGet("followers/{IDUser}")]
        public async Task<IActionResult> GetUserFollowers([FromRoute] long IDUser) // Get followers of a user
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<FollowerResponse> followers = await _followerService.GetUserFollowers(IDUser);
            return Ok(new { IDUser, followers, count = followers.Count });
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetUserFeed() // Get user feed
        {
            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<PostResponse> feed = await _followerService.GetUserFeed(validateTokenResponse.IDUser);
            return Ok(new { feed });
        }
    }
}
