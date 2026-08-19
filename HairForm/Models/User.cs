namespace HairForm.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }

    public class LoginViewModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public enum Role
    {
        Admin =1,
        User =2,
    }
}
