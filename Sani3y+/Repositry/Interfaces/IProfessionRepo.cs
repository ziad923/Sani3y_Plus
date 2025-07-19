using Microsoft.AspNetCore.Mvc;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.Home;
using Sani3y_.Dtos.User;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IProfessionRepo
	{
		Task<List<ProfessionDto>> GetAllAsync();
		Task<List<ExploreProfessionDto>> GetProfessionsWithImagesAsync();

        Task<Profession?> GetByNameAsync(string name);
		Task<Profession> AddAsync(ProfessionCreateOrUpdateDto profession);
		Task<bool> UpdateAsync(int id, ProfessionCreateOrUpdateDto professionDto);
		Task<bool> DeleteAsync(int id);
	}
}
