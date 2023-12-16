using System.Text.Json.Serialization;

namespace OperationService.Shared.DTO.Response
{
    public class LikeResponse
    {
        public long IDLike { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long IDUser { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long IDPost { get; set; }

        public DateTime Date { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public PostResponse Post { get; set; }
    }
}
