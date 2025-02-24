using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Selu383.SP25.P02.Test.Controllers.Authentication;
using Selu383.SP25.P02.Test.Dtos;
using Selu383.SP25.P02.Test.Helpers;

namespace Selu383.SP25.P02.Test.Controllers.Users;

[TestClass]
public class UsersControllerTests
{
    private WebTestContext context = new();

    [TestInitialize]
    public void Init()
    {
        context = new WebTestContext();
    }

    [TestCleanup]
    public void Cleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task CreateUser_NotLoggedIn_Returns401()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = GetNewUser();

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [TestMethod]
    public async Task CreateUser_EmptyUsername_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewUser();
        target.UserName = "";

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUser_DuplicateUsername_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewUser();
        target.UserName = "bob";

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUser_NoSuchRole_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewUser();
        target.Roles = new[] { "NotARole" };

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUser_EmptyRole_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewUser();
        target.Roles = Array.Empty<string>();

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUser_NoPassword_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewUser();
        target.Password = string.Empty;

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUser_BadPassword_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewUser();
        target.Password = "password";

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateUser_ValidUserWhileLoggedIn_Returns200()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewUser();

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK );
        var createdUser = await httpResponse.Content.ReadAsJsonAsync<UserDto>();
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(new
        {
            target.UserName,
            target.Roles
        });

        var loginAsThatUser = await webClient.PostAsJsonAsync("/api/authentication/login", new LoginDto
        {
            UserName = target.UserName,
            Password = target.Password
        });
        var loginResult = await loginAsThatUser.AssertLoginFunctions();
        loginResult.Should().BeEquivalentTo(createdUser, "we expect our created user to match our login");
    }

    [TestMethod]
    public async Task CreateUser_ValidAdminWhileLoggedIn_Returns200()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var currentLogin = await webClient.LoginAsAdminAsync();
        if (currentLogin == null)
        {
            Assert.Fail();
            return;
        }
        var target = GetNewAdmin();

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/users", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdUser = await httpResponse.Content.ReadAsJsonAsync<UserDto>();
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(new
        {
            target.UserName,
            target.Roles
        } );

        var loginAsThatUser = await webClient.PostAsJsonAsync("/api/authentication/login", new LoginDto
        {
            UserName = target.UserName,
            Password = target.Password
        });
        var loginResult = await loginAsThatUser.AssertLoginFunctions();
        loginResult.Should().BeEquivalentTo(createdUser);
    }

    private static CreateUserDto GetNewUser()
    {
        return new CreateUserDto
        {
            UserName = Guid.NewGuid().ToString("N"),
            Password = Guid.NewGuid().ToString("N") + "aSd!@#",
            Roles = new[] { "User" }
        };
    }

    private static CreateUserDto GetNewAdmin()
    {
        return new CreateUserDto
        {
            UserName = Guid.NewGuid().ToString("N"),
            Password = Guid.NewGuid().ToString("N") + "aSd!@#",
            Roles = new[] { "Admin" }
        };
    }
}
