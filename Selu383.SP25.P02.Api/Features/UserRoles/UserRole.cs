using Selu383.SP25.P02.Api.Features.Theaters.Roles;
using Selu383.SP25.P02.Api.Features.Users;

namespace Selu383.SP25.P02.Api.Features.UserRoles
{
    public class UserRole
    {
        public int UserId { get; set; }
        public virtual required User User { get; set; }
        public int RoleId { get; set; }
        public virtual required Role Role { get; set; }

        // Navigation properties to parent entities
    }
}
