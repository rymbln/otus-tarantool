
using WebApplication1.Model;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface ITarantoolService
    {
        Task<int> Init();
        Task<UserTar?> GetById(long id);
        Task<UserTar?> GetByEmail(string email);
        Task Insert(ICollection<UserTar> items);
    }
}