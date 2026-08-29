using System;
using System.Collections.Generic;
using System.Text;

namespace CleanAuthDemo.Application.Authentication
{
    public sealed record AuthenticatedUser(
        Guid Id,
        string Email,
        IEnumerable<string> Roles,
        IEnumerable<string> Permissions
    );
}
