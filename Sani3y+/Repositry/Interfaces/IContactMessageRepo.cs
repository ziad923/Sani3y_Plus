using Sani3y_.Dtos;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IContactMessageRepo
    {
        Task<List<ContactUsResponseDto>> GetContactMessagesAsync(bool onlyUnresolved = true);
        Task<ContactUs?> GetContactMessageByRequestNumberAsync(string requestNumber);
        Task UpdateAsync(ContactUs contactMessage);
    }
}
