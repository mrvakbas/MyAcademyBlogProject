using Blogy.Business.DTOs.Common;

namespace Blogy.Business.DTOs.ContactDtos
{
    public class UpdateContactDto : BaseDto
	{
		public string NameSurname { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
	}
}
