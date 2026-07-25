namespace PruebaTecnica.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int? UsuarioId { get; set; }
}