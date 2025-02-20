namespace Selu383.SP25.P02.Api.Features.Users
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
