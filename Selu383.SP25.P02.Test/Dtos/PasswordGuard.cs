namespace Selu383.SP25.P02.Test.Dtos;

internal class PasswordGuard
{
    public string? Password
    {
        get => null;
        set => throw new Exception("You returned a password, don't do this!");
    }

    public string? PasswordHash
    {
        get => null;
        set => throw new Exception("You returned a password, don't do this!");
    }
}