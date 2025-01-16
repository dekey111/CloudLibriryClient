namespace Server.Models
{
    public class UserResponse
    {
        public UserResponse() { }

        public int IdUser { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int IdRole { get; set; }
    }
}
