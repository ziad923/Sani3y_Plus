using Sani3y_.Dtos.Admin;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Dtos.User;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IUserRepo
    {
        Task<RatingResponseDto> AddRatingAsync(string userId, string craftsmanId, UserRatingDto ratingDto); // IUserRatingRepository
        // IUserRatingRepository
        // IUserProfileRepository // IServiceRequestRepository
        // IUserProfileRepository// IServiceRequestRepository
       // IUserRatingRepository // IServiceRequestRepository
        // IServiceRequestRepository // IServiceRequestRepository
        Task<CraftsmanCardDto> GetCraftsmanProfileByIdAsync(string craftsmanId);
        Task<CraftsmanRatingsResponseDto> GetCraftsmanRatingsAsync(string craftsmanId); // IRatingRepository
        Task<ServiceResponseDto> CreateServiceRequestAsync(string userId, ServiceRequestDto dto);
        Task<bool> EditServiceRequestAsync(string requestId, string userId, ServiceRequestDto dto);
        Task<bool> CancelServiceRequestAsync(string requestId, string userId);
        Task<bool> MarkRequestAsCompleteAsync(string requestId, string userId);

        // Task<ServiceRequest?> GetServiceRequestByIdAsync(string requestId);
        Task<List<GetAllRequestsUser>> GetAllRequestsAsync(string userId);
        Task<List<CraftsmanPreviousWorkDto>> GetCraftsmanPreviousWorkAsync(string craftsmanId);
        // Task AddServiceRequestAsync(ServiceRequest request);
        //  Task<bool> UpdateServiceRequestAsync(ServiceRequest request);
        Task<List<UserListDto>> GetAllUsersAsync();
        Task<List<CraftsmanListDto>> GetAllCraftsmenAsync();
        Task<bool> DeleteUserAsync(string userId);
    }
}
