namespace PasswordManager.Database.Tenet.Models;

public class TenetUser
{
    public TenantId Id { get; set; }

    public TenetUser(TenantId id)
    {
        Id = id;
    }

    private TenetUser()
    {
        
    }
}

public class TenantId:IParsable<TenantId>
{
    public Guid Id { get;  set; }
    public DateTime CreatedDate { get; set; }

    public TenantId(Guid id, DateTime createdDate)
    {
        Id = id;
        CreatedDate = createdDate;
    }

    public static TenantId Parse(string s, IFormatProvider? provider)
    {
        var data = s.Split(",");

        return new TenantId(Guid.Parse(data[0]), DateTime.Parse(data[1]));
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out TenantId result)
    {
        var data = s.Split(",");
        if (data.Length != 2)
        {
            result = null!;
            return false;
        }
        result=new TenantId(Guid.Parse(data[0]), DateTime.Parse(data[1]));
        return true;
    }
};