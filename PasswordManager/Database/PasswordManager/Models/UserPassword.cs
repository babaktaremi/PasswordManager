namespace PasswordManager.Database.PasswordManager.Models;

public class UserPassword
{
    public Guid Id { get; set; }
    public string Password { get; set; }

    public UserPassword(Guid id, string password)
    {
        Id = id;
        Password = password;
    }

    private UserPassword()
    {
        
    }
}