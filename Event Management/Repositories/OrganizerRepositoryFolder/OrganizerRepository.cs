using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.OrganizerRepositoryFolder
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly DataContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public OrganizerRepository(DataContext context, IImageRepository imageRepository, IMapper mapper)
        {
            _context = context;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrganizerDto>> GetOrganizersAsync()
        {
            var organizers = await _context.Organizers
                .Include(x => x.Events)
                .Include(x => x.Locations)
                .Include(x => x.User)
                .ToListAsync();

            var organizerDtos = _mapper.Map<IEnumerable<OrganizerDto>>(organizers);

            return organizerDtos;
        }

        public async Task<OrganizerDto> GetOrganizerByIdAsync(int id)
        {
            var organizer = await _context.Organizers
                .Include(x => x.Events)
                .Include(x => x.Locations)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            var organizerDto = _mapper.Map<OrganizerDto>(organizer);

            return organizerDto;
        }

        public async Task<OrganizerDto> AddOrganizerAsync(OrganizerCreateDto organizerCreateDto)
        {
            var organizer = _mapper.Map<Organizer>(organizerCreateDto);

            var logoUrl = await _imageRepository.GenerateImageSource(organizerCreateDto.Logo);
            organizer.LogoUrl = logoUrl;

            await _context.Organizers.AddAsync(organizer);
            await _context.SaveChangesAsync();

            var organizerDto = _mapper.Map<OrganizerDto>(organizer);
            return organizerDto;
        }

        public async Task<bool> UpdateOrganizerAsync(int id, OrganizerUpdateDto organizerUpdateDto)
        {
            var existingOrganizer = await _context.Organizers.FirstOrDefaultAsync(o => o.Id == id);
            if (existingOrganizer == null) return false;

            var organizer = _mapper.Map(organizerUpdateDto, existingOrganizer);

            _context.Organizers.Update(organizer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrganizerAsync(int id)
        {
            var organizer = await _context.Organizers.FindAsync(id);
            if (organizer == null) return false;

            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
