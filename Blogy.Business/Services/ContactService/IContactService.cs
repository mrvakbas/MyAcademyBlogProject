using Blogy.Business.DTOs.ContactDtos;

namespace Blogy.Business.Services.ContactService
{
    public interface IContactService : IGenericService<ResultContactDto,UpdateContactDto,CreateContactDto>
	{
    }
}
