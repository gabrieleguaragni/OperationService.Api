using Microsoft.AspNetCore.Mvc;
using OperationService.Business.Abstractions.Services;
using OperationService.Shared.DTO.Response;

namespace OperationService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILikeService _likeService;

        public LikeController(
            IAuthService authService,
            ILikeService likeService
            )
        {
            _authService = authService;
            _likeService = likeService;
        }

        [HttpGet("{IDPost}")]
        public async Task<IActionResult> GetLikeStatus([FromRoute] long IDPost) // Get like status
        {
            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            bool status = await _likeService.GetLikeStatus(validateTokenResponse.IDUser, IDPost);
            return Ok(new { message = status ? "Liked" : "Not liked", status });
        }

        [HttpPost("toggle/{IDPost}")]
        public async Task<IActionResult> ToggleLike([FromRoute] long IDPost) // Add or remove like
        {
            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            string result = await _likeService.ToggleLike(validateTokenResponse.IDUser, IDPost);
            return Ok(new { message = result });
        }

        [HttpGet("user/{IDUser}")]
        public async Task<IActionResult> GetUserLikes([FromRoute] long IDUser) // Get all liked post from a user
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<LikeResponse> likes = await _likeService.GetUserLikes(IDUser);
            return Ok(new { IDUser, likes, count = likes.Count });
        }

        [HttpGet("post/{IDPost}")]
        public async Task<IActionResult> GetPostLikes([FromRoute] long IDPost) // Get all likes from a post
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<LikeResponse> likes = await _likeService.GetPostLikes(IDPost);
            return Ok(new { IDPost, likes, count = likes.Count });
        }
    }
}
