namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Task<string> GetUserName();
        Task<bool> IsInAuthorizedGroup();
    }
}
