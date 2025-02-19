namespace Selu383.SP25.P02.Api.Features.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        // Navigation property for many-to-many relationship with Role
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }
    }
}
