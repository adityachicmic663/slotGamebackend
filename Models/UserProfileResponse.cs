namespace SlotGameBackend.Models
{
    public class UserProfileResponse
    {
        public Guid userId {  get; set; }   

        public string userName {  get; set; }   

        public string firstName {  get; set; }  

        public string lastName { get; set; }

        public string role {  get; set; }

        public string email { get; set; }

        public int walletBalance {  get; set; }

    }
}
