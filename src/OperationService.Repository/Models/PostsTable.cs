namespace OperationService.Repository.Models
{
    public class PostsTable
    {
        public long IDPost { get; set; }

        public long IDUser { get; set; }

        public string Image { get; set; }

        public string? Text { get; set; }

        public DateTime Date { get; set; }

        public List<LikesTable> Likes { get; set; }

        public List<CommentsTable> Comments { get; set; }
    }
}
