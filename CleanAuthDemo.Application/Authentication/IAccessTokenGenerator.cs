namespace CleanAuthDemo.Application.Authentication;

public interface IAccessTokenGenerator
{
    AccessTokenResult Generate(AuthenticatedUser user);
}