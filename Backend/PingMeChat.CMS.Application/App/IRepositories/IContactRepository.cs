using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.App.IRepositories
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<IEnumerable<Contact>> GetContactsByUserIdAsync(string userId);
    }
}
