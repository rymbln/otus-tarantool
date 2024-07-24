using NpgsqlTypes;

using ProGaudi.MsgPack.Light;

using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Model;

public class User
{
    public User()
    {
    }

    public User(int id, string firstName, string lastName, string email, string gender, string ipAddress, bool isActive, DateTime birthdate, int score, Guid uniqueId, DateTime created)
    {
        Id = id;
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Gender = gender ?? throw new ArgumentNullException(nameof(gender));
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        IsActive = isActive;
        Birthdate = birthdate;
        Score = score;
        UniqueId = uniqueId;
        Created = created;
    }

    [Column(Order = 1)]
    public int Id { get; set; }

    [Column(Order = 2)]
    public string FirstName { get; set; }

    [Column(Order = 3)]
    public string LastName { get; set; }

    [Column(Order = 4)]
    public string Email { get; set; }

    [Column(Order = 5)]
    public string Gender { get; set; }

    [Column(Order = 6)]
    public DateTime Birthdate { get; set; }

    [Column(Order = 7)]
    public bool IsActive { get; set; }

    [Column(Order = 8)]
    public string IpAddress { get; set; }

    [Column(Order = 9)]
    public int Score { get; set; }

    [Column(Order = 10)]
    public DateTime Created { get; set; }

    [Column(Order = 11)]
    public Guid UniqueId { get; set; }
}
