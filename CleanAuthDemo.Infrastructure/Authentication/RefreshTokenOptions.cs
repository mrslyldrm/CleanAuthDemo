using System;
using System.Collections.Generic;
using System.Text;

namespace CleanAuthDemo.Infrastructure.Authentication
{
    public sealed class RefreshTokenOptions
    {
        public const string SectionName = "RefreshToken";
        public int ExpirationDays { get; set; } = 14;
    }
}
