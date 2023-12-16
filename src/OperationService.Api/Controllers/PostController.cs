using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OperationService.Business.Abstractions.Services;
using OperationService.Shared.DTO.Request;
using OperationService.Shared.DTO.Response;

namespace OperationService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPostService _postService;
        private IValidator<IFormFile> _imageValidator;
        private readonly IValidator<string> _textValidator;

        public PostController(
            IAuthService authService,
            IPostService postService,
            IValidator<IFormFile> imageValidator,
            IValidator<string> textValidator
            )
        {
            _authService = authService;
            _postService = postService;
            _imageValidator = imageValidator;
            _textValidator = textValidator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] IFormFile file, [FromForm] string text) // Add new post
        {
            var validationResultImage = await _imageValidator.ValidateAsync(file);
            if (!validationResultImage.IsValid)
            {
                return StatusCode(400, new { message = validationResultImage.Errors.First().ErrorMessage });
            }

            var validationResultText = await _textValidator.ValidateAsync(text);
            if (!validationResultText.IsValid)
            {
                return StatusCode(400, new { message = validationResultText.Errors.First().ErrorMessage });
            }

            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            PostResponse post = await _postService.CreatePost(validateTokenResponse.IDUser, file, text);
            return Ok(new { message = "Post added successfully", post });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostRequest updatePostRequest) // Update existing post
        {
            var validationResultText = await _textValidator.ValidateAsync(updatePostRequest.Text);
            if (!validationResultText.IsValid)
            {
                return StatusCode(400, new { message = validationResultText.Errors.First().ErrorMessage });
            }

            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            PostResponse post = await _postService.UpdatePost(validateTokenResponse.IDUser, validateTokenResponse.Roles, updatePostRequest.IDPost, updatePostRequest.Text);
            return Ok(new { message = "Post updated successfully", post });
        }

        [HttpPost("delete/{IDPost}")]
        public async Task<IActionResult> DeletePost([FromRoute] long IDPost) // Delete comment
        {
            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            await _postService.DeletePost(validateTokenResponse.IDUser, validateTokenResponse.Roles, IDPost);
            return Ok(new {message = "Post deleted" });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetPosts() // Get all posts
        {
            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<PostResponse> posts = await _postService.GetPosts(validateTokenResponse.IDUser);
            return Ok(new { posts });
        }

        [HttpGet("list/{IDUser}")]
        public async Task<IActionResult> GetPostsByIDUser([FromRoute] long IDUser) // Get all posts from a user
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<PostResponse> posts = await _postService.GetPosts(IDUser);
            return Ok(new { posts });
        }

        [HttpGet("{IDPost}")]
        public async Task<IActionResult> GetPostInfo([FromRoute] long IDPost) // Get post details
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            PostResponse post = await _postService.GetPost(IDPost);
            return Ok(new { post });
        }
    }
}