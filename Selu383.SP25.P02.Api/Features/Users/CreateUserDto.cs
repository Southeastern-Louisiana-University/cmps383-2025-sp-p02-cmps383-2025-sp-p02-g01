namespace Selu383.SP25.P02.Api.Features.Users
{
    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
