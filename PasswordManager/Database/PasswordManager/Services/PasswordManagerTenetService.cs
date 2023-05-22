namespace PasswordManager.Database.PasswordManager.Services;

public class PasswordManagerTenetService:IPasswordManagerTenetService
{
    public Guid UserId { get; set; }
    public void SetUserTenetId(Guid userId)
    {
        this.UserId = userId;
    }

    public Guid GetUserTenetId()
    {
        return UserId;
    }
}