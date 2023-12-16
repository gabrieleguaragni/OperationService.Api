namespace OperationService.Repository.Models
{
    public class LikesTable
    {
        public long IDLike { get; set; }

        public long IDUser { get; set; }

        public long IDPost { get; set; }

        public DateTime Date { get; set; }

        public PostsTable Post { get; set; }
    }
}
