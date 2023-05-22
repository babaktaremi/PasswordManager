namespace PasswordManager.Database.PasswordManager.Services;

public interface IPasswordManagerTenetService
{
    public Guid UserId { get;protected set; }

    void SetUserTenetId(Guid userId);
    Guid GetUserTenetId();
}