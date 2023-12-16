namespace OperationService.Repository.Models
{
    public class FollowersTable
    {
        public long IDFollower { get; set; }

        public long IDUser { get; set; }

        public long IDFollow { get; set; }

        public DateTime Date { get; set; }
    }
}
