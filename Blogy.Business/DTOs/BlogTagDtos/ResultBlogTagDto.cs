using Blogy.Business.DTOs.Common;
using Blogy.Entity.Entities;

namespace Blogy.Business.DTOs.BlogTagDtos
{
	public class ResultBlogTagDto : BaseDto
	{
		public int BlogId { get; set; }
		public int TagId { get; set; }
		public Blog Blog { get; set; }
		public Tag Tag { get; set; }
	}
}
