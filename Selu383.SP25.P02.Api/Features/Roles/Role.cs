using Selu383.SP25.P02.Api.Features.UserRoles;

namespace Selu383.SP25.P02.Api.Features.Theaters.Roles
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property for many-to-many relationship with User
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }
    }
}
