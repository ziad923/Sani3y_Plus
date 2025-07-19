using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.Home;
using Sani3y_.Dtos.User;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class ProfessionRepository : IProfessionRepo
	{
		private readonly AppDbContext _context;
		private readonly IFileService _fileService;

		public ProfessionRepository(AppDbContext context, IFileService fileService)
		{
			_context = context;
			_fileService = fileService;
		}

		public async Task<List<ProfessionDto>> GetAllAsync()
		{
			return await _context.Professions
			  .AsNoTracking()
			  .Select(p => new ProfessionDto { Id = p.Id, Name = p.Name })
			  .ToListAsync();
		}
        public async Task<List<ExploreProfessionDto>> GetProfessionsWithImagesAsync()
        {
          return  await _context.Professions
                 .AsNoTracking()
                 .Select(p => new ExploreProfessionDto
                 {
                     Id = p.Id,
                     Name = p.Name,
                     ImagePath = p.ImagePath
                 }).ToListAsync();
        }
        public async Task<Profession?> GetByNameAsync(string name)
		{
			return await _context.Professions.FirstOrDefaultAsync(p => p.Name == name);
		}

		public async Task<Profession> AddAsync(ProfessionCreateOrUpdateDto professionDto)
		{
			var profession = new Profession
			{
				Name = professionDto.Name,
				ImagePath = null
			};

			if (professionDto.ImagePath != null)
			{
				profession.ImagePath = await _fileService.SavePictureAsync(professionDto.ImagePath);
			}

			await _context.Professions.AddAsync(profession);
			await _context.SaveChangesAsync();

			return profession;
		}

		public async Task<bool> UpdateAsync(int id, ProfessionCreateOrUpdateDto professionDto)
		{
			var profession = await _context.Professions.FindAsync(id);
			if (profession == null) 
				 return false;
			profession.Name = professionDto.Name;

			if (professionDto.ImagePath != null)
			{
				// Delete old image
				if (!string.IsNullOrEmpty(profession.ImagePath))
				{
					_fileService.DeleteFile(profession.ImagePath);
				}

				// Save new image
				profession.ImagePath = await _fileService.SavePictureAsync(professionDto.ImagePath);
			}
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var profession = await _context.Professions.FindAsync(id);
			if (profession is null)
				return false;

			// Delete image if it exists
			if (!string.IsNullOrEmpty(profession.ImagePath))
			{
				_fileService.DeleteFile(profession.ImagePath);
			}

			_context.Professions.Remove(profession);
			await _context.SaveChangesAsync();
			return true;
		}
    }
}
