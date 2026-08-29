namespace CleanAuthDemo.Application.Authentication;

public enum RefreshTokenRotationStatus
{
    Success,
    Invalid,
    Expired,
    Reused
}