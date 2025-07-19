using Sani3y_.Dtos.Craftman;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ICraftsmanWorkRepo
    {
        Task AddPreviousWorkAsync(string craftsmanId, PreviousWorkDto previousWorkDto);
        Task<List<PreviousWorkResponseDto>> GetPreviousWork(string craftsmanId);
        Task<bool> UpdatePreviousWorkAsync(int id, PreviousWorkDto dto, string craftsmanId);
        Task<bool> DeletePreviousWorkAsync(int id, string craftsmanId);
    }
}
