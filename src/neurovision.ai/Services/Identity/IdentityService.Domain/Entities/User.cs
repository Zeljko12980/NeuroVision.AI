namespace IdentityService.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

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
