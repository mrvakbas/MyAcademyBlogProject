using Blogy.Business.DTOs.Common;

namespace Blogy.Business.DTOs.ContactDtos
{
    public class ResultContactDto : BaseDto
	{
		public string NameSurname { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
	}
}
