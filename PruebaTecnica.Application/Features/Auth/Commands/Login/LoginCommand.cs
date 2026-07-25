using MediatR;

namespace PruebaTecnica.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string NombreUsuario,
    string Contrasenia
) : IRequest<LoginResult>;