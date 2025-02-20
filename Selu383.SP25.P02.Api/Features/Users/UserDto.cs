namespace Selu383.SP25.P02.Api.Features.Users
{
    public class UserDto
    {

        public int Id { get; set; }
        public string UserName { get; set; }
        public List<Roles.RoleDto> Roles { get; set; } = new List<Roles.RoleDto>();
    }
}
