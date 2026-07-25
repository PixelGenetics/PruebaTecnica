using PruebaTecnica.Application.Features.Auth.DTOs;

namespace PruebaTecnica.Application.Features.Auth.Commands.Login;

public class LoginResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public LoginResponseDto? Data { get; set; }
}