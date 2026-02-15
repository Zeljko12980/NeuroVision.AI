namespace IdentityService.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;

        protected User() { }

        public User(string username, string email, string firstName, string lastName)
        {
            UserName = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public void UpdateName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
