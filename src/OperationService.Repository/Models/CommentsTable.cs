namespace OperationService.Repository.Models
{
    public class CommentsTable
    {
        public long IDComment { get; set; }

        public long IDUser { get; set; }

        public long IDPost { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public PostsTable Post { get; set; }
    }
}
