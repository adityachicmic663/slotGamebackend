namespace SlotGameBackend.Models
{
    public class UserResponse
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public bool isBlocked {  get; set; }
    }
}
