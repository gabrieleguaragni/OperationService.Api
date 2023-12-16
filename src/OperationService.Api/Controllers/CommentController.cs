using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OperationService.Business.Abstractions.Services;
using OperationService.Shared.DTO.Request;
using OperationService.Shared.DTO.Response;

namespace OperationService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICommentService _commentService;
        private readonly IValidator<string> _textValidator;

        public CommentController(
            IAuthService authService,
            ICommentService commentService,
            IValidator<string> textValidator
            )
        {
            _authService = authService;
            _commentService = commentService;
            _textValidator = textValidator;
        }

        [HttpGet("{IDComment}")] 
        public async Task<IActionResult> GetComment([FromRoute] long IDComment) // Get comment details
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            CommentResponse comment = await _commentService.GetComment(IDComment);
            return Ok(new { comment });
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertComment([FromBody] InsertCommentRequest insertCommentRequest) // Add new comment
        {
            var validationResult = await _textValidator.ValidateAsync(insertCommentRequest.Text);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, new { message = validationResult.Errors.First().ErrorMessage });
            }

            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            CommentResponse comment = await _commentService.InsertComment(validateTokenResponse.IDUser, insertCommentRequest.IDPost, insertCommentRequest.Text);
            return Ok(new { message = "Comment added successfully", comment });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequest updateCommentRequest) // Update existing comment
        {
            var validationResult = await _textValidator.ValidateAsync(updateCommentRequest.Text);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, new { message = validationResult.Errors.First().ErrorMessage });
            }

            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            CommentResponse comment = await _commentService.UpdateComment(validateTokenResponse.IDUser, validateTokenResponse.Roles, updateCommentRequest.IDComment, updateCommentRequest.Text);
            return Ok(new { message = "Comment updated successfully", comment});
        }

        [HttpPost("delete/{IDComment}")]
        public async Task<IActionResult> DeleteComment([FromRoute] long IDComment) // Delete comment
        {
            ValidateTokenResponse validateTokenResponse = await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            await _commentService.DeleteComment(validateTokenResponse.IDUser, validateTokenResponse.Roles, IDComment);
            return Ok(new { message = "Comment deleted successfully" });
        }

        [HttpGet("user/{IDUser}")]
        public async Task<IActionResult> UserComments([FromRoute] long IDUser) // Get all comments from a user
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<CommentResponse> comments = await _commentService.GetUserComments(IDUser);
            return Ok(new { comments });
        }

        [HttpGet("post/{IDPost}")]
        public async Task<IActionResult> PostComments([FromRoute] long IDPost) // Get all comments of a post
        {
            await _authService.TokenValidation(HttpContext.Request.Headers.Authorization);
            List<CommentResponse> comments = await _commentService.GetPostComments(IDPost);
            return Ok(new { comments });
        }
    }
}
