using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

public class AuthenticateUserResponseProfile : Profile
{
    public AuthenticateUserResponseProfile()
    {
        CreateMap<AuthenticateUserResult, AuthenticateUserResponse>();
    }
}