namespace PasswordManager.Database.Tenet.Models;

public class TenetUser
{
    public Guid Id { get; set; }

    public TenetUser(Guid id)
    {
        Id = id;
    }

    private TenetUser()
    {
        
    }
}