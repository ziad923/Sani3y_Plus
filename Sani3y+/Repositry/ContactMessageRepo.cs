using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class ContactMessageRepo : IContactMessageRepo
    {
        private readonly AppDbContext _context;
        public ContactMessageRepo(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<List<ContactUsResponseDto>> GetContactMessagesAsync(bool onlyUnresolved = true)
        {
            IQueryable<ContactUs> query = _context.ContactMessages.Include(c => c.User);

            if (onlyUnresolved)
            {
                query = query.Where(c => !c.IsResolved);
            }

           return await query
                .OrderByDescending(c => c.SentAt)
                .Select(c => new ContactUsResponseDto
                {
                    RequestNumber = c.RequestNumber,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    MessageContent = c.MessageContent,
                    SentAt = c.SentAt,
                    IsResolved = c.IsResolved,
                    ResolvedAt = c.ResolvedAt,
                    UserId = c.UserId
                })
                .ToListAsync();
        }
        public async Task<ContactUs?> GetContactMessageByRequestNumberAsync(string requestNumber)
        {
            return await _context.ContactMessages
                .FirstOrDefaultAsync(c => c.RequestNumber == requestNumber);
        }

        public async Task UpdateAsync(ContactUs contactMessage)
        {
            _context.ContactMessages.Update(contactMessage);
            await _context.SaveChangesAsync();
        }
    }
}
