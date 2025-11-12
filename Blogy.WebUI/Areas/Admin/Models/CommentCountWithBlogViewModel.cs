namespace Blogy.WebUI.Areas.Admin.Models
{
	public class CommentCountWithBlogViewModel
	{
		public string BlogTitle { get; set; }
		public int CommentCount { get; set; }
        public int BlogId { get; set; }
    }
}