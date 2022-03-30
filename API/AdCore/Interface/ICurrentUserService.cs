namespace AdCore.Interface
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
    }
}
