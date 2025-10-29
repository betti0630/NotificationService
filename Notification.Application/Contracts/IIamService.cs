namespace Notification.Application.Contracts;

public interface IIamService
{
    Task<UserData?> GetUserDataByUserId(int userId, CancellationToken ct = default);
}

public class UserData
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}