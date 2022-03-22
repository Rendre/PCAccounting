namespace SharedKernel.Services;

public interface ILoginService
{
    public bool IsSessionValid(string? token);
}