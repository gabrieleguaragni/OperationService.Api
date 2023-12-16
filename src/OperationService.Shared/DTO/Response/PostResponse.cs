using System.Text.Json.Serialization;

namespace OperationService.Shared.DTO.Response
{
    public class PostResponse
    {
        public long IDPost { get; set; }

        public long IDUser { get; set; }

        public string Image { get; set; }

        public string? Text { get; set; }

        public DateTime Date { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<LikeResponse> Likes { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<CommentResponse> Comments { get; set; }
    }
}
