namespace Blogy.Business.DTOs.ContactDtos
{
    public class CreateContactDto
	{
		public string NameSurname { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
	}
}
