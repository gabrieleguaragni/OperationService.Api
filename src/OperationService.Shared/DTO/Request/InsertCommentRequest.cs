namespace OperationService.Shared.DTO.Request
{
    public class InsertCommentRequest
    {
        public long IDPost { get; set; }

        public string Text { get; set; }
    }
}
