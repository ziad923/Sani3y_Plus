using Sani3y_.Dtos.Verification;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IVerificationService
    {
        Task SubmitVerificationRequestAsync(string craftsmanId, SubmitVerificationRequestDto dto);
        Task<List<VerificationRequestResponseDto>> GetPendingRequestsAsync();
        Task<bool> ApproveRequestAsync(int requestId);
        Task<bool> RejectRequestAsync(int requestId);
    }
}
