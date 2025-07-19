using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class CraftsmanWorkRepo : ICraftsmanWorkRepo
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public CraftsmanWorkRepo(AppDbContext appDbContext, IFileService fileService)
        {
            _context = appDbContext;
           _fileService = fileService;
        }
        public async Task AddPreviousWorkAsync(string craftsmanId, PreviousWorkDto previousWorkDto)
        {
            var previousWork = new PreviousWork
            {
                ProjectDescription = previousWorkDto.ProjectDescription,
                DateJobDone = previousWorkDto.DateJobDone,
                CraftsmanId = craftsmanId
            };
            if (previousWorkDto.Pictures != null && previousWorkDto.Pictures.Any())
            {
                foreach (var picture in previousWorkDto.Pictures)
                {
                    // Save the picture to the file system or cloud (example: save to local folder)
                    var pictureUrl = await _fileService.SavePictureAsync(picture);
                    previousWork.PictureUrls.Add(pictureUrl);
                }
            }

            // Add the previous work to the database
            _context.PreviousWorks.Add(previousWork);
            await _context.SaveChangesAsync();
        }
        public async Task<List<PreviousWorkResponseDto>> GetPreviousWork(string craftsmanId)
        {
            return await _context.PreviousWorks
                   .Where(pw => pw.CraftsmanId == craftsmanId)
                   .Select(pw => new PreviousWorkResponseDto
                   {
                       Id = pw.Id,
                       ProjectDescription = pw.ProjectDescription,
                       DateJobDone = pw.DateJobDone,
                       PictureUrls = pw.PictureUrls
                   }).ToListAsync();
        }
        public async Task<bool> UpdatePreviousWorkAsync(int id, PreviousWorkDto dto, string craftsmanId)
        {
            var work = await _context.PreviousWorks.FirstOrDefaultAsync(pw => pw.Id == id && pw.CraftsmanId == craftsmanId);
            if (work == null) return false;

            work.ProjectDescription = dto.ProjectDescription;
            work.DateJobDone = dto.DateJobDone;

            if (dto.Pictures != null && dto.Pictures.Any())
            {
                foreach (var picture in dto.Pictures)
                {
                    var url = await _fileService.SavePictureAsync(picture);
                    work.PictureUrls.Add(url);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePreviousWorkAsync(int id, string craftsmanId)
        {
            var work = await _context.PreviousWorks.FirstOrDefaultAsync(pw => pw.Id == id && pw.CraftsmanId == craftsmanId);
            if (work == null) return false;

            _context.PreviousWorks.Remove(work);
            await _context.SaveChangesAsync();
            return true;
        }
        
    }
}
