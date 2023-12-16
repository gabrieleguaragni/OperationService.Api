using System.Text.Json.Serialization;

namespace OperationService.Shared.DTO.Response
{
    public class CommentResponse
    {
        public long IDComment { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long IDUser { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long IDPost { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public PostResponse Post { get; set; }
    }
}
