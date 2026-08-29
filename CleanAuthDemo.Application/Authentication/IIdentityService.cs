using System;
using System.Collections.Generic;
using System.Text;

namespace CleanAuthDemo.Application.Authentication
{
    public interface IIdentityService
    {
        Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthenticatedUser?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
