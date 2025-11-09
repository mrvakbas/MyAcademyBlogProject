using AutoMapper;
using Blogy.Business.DTOs.ContactDtos;
using Blogy.DataAccess.Repositories.ContactRepository;
using Blogy.Entity.Entities;

namespace Blogy.Business.Services.ContactService
{
    public class ContactService(IContactRepository _contactRepository, IMapper _mapper) : IContactService
    {
        public async Task<List<ResultContactDto>> GetAllAsync()
        {
			var values = await _contactRepository.GetAllAsync();
			return _mapper.Map<List<ResultContactDto>>(values);
		}

        public async Task<UpdateContactDto> GetByIdAsync(int id)
        {
			var value = await _contactRepository.GetByIdAsync(id);
			return _mapper.Map<UpdateContactDto>(value);
		}

        public async Task<ResultContactDto> GetSingleByIdAsync(int id)
        {
			var value = await _contactRepository.GetByIdAsync(id);
			return _mapper.Map<ResultContactDto>(value);
		}

        public async Task CreateAsync(CreateContactDto dto)
        {
			var entity = _mapper.Map<Contact>(dto);
			await _contactRepository.CreateAsync(entity);
		}

        public async Task UpdateAsync(UpdateContactDto dto)
        {
			var entity = _mapper.Map<Contact>(dto);
			await _contactRepository.UpdateAsync(entity);
		}

        public async Task DeleteAsync(int id)
        {
			await _contactRepository.DeleteAsync(id);
		}
    }
}
