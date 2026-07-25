using MediatR;

namespace PruebaTecnica.Application.Features.Auth.Commands.RegisterUser;

public record RegisterUserCommand(
    string NombreUsuario,
    string Contrasenia,
    string Nombre,
    string Correo
) : IRequest<RegisterUserResult>;