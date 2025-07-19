using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Verification;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;

        public VerificationService(AppDbContext context, IFileService fileService, INotificationService notificationService)
        {
            _context = context;
            _fileService = fileService;
            _notificationService = notificationService;
        }
        public async Task SubmitVerificationRequestAsync(string craftsmanId, SubmitVerificationRequestDto dto)
        {
            // Check if craftsman exists
            var craftsman = await _context.Users.FindAsync(craftsmanId);
            if (craftsman == null)
                throw new Exception("الحرفي غير موجود.");

            // Check if already verified
            if (craftsman.IsTrusted == true)
                throw new Exception("هذا الحساب موثق بالفعل.");

            // Check if a pending request exists
            bool hasPendingRequest = await _context.VerificationRequests
                .AnyAsync(r => r.CraftsmanId == craftsmanId && !r.IsApproved && !r.IsRejected);

            if (hasPendingRequest)
                throw new Exception("لديك طلب توثيق قيد المراجعة بالفعل.");

            var profilePath = await _fileService.SavePictureAsync(dto.ProfileImage);
            var cardPath = await _fileService.SavePictureAsync(dto.CardImage);

            var request = new VerificationRequest
            {
                CraftsmanId = craftsmanId,
                ProfileImagePath = profilePath,
                CardImagePath = cardPath,
                SubmittedAt = DateTime.UtcNow,
            };

            _context.VerificationRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<VerificationRequestResponseDto>> GetPendingRequestsAsync()
        {
            return await _context.VerificationRequests
                .Where(r => !r.IsApproved && !r.IsRejected)
                .Include(r => r.Craftsman)
                .Select(r => new VerificationRequestResponseDto
                {
                    Id = r.Id,
                    CraftsmanId = r.CraftsmanId,
                    FullName = r.Craftsman.FirstName + " " + r.Craftsman.LastName,
                    ProfileImagePath = r.ProfileImagePath,
                    CardImagePath = r.CardImagePath,
                    SubmittedAt = r.SubmittedAt
                })
                .ToListAsync();
        }

        public async Task<bool> ApproveRequestAsync(int requestId)
        {
            var request = await _context.VerificationRequests
                .Include(r => r.Craftsman)
                .FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null) return false;

            request.IsApproved = true;
            request.Craftsman.IsTrusted = true;

            await _notificationService.SendNotificationAsync(request.CraftsmanId,
                "تم توثيق حسابك",
                "تهانينا! تم توثيق حسابك بنجاح من قبل الإدارة.");

            await _context.SaveChangesAsync(); 
            return true; 
        }

        public async Task<bool> RejectRequestAsync(int requestId)
        {
            var request = await _context.VerificationRequests
                .Include(r => r.Craftsman)
                .FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null) return false;

            request.IsRejected = true;

            await _notificationService.SendNotificationAsync(request.CraftsmanId,
                "تم رفض التوثيق",
                "نأسف، لم يتم قبول طلب توثيق حسابك. يرجى مراجعة البيانات المرسلة والمحاولة مرة أخرى.");

            await _context.SaveChangesAsync();
            return true;  
        }
    }
}
