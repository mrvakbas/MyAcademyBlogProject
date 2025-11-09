using Blogy.DataAccess.Context;
using Blogy.DataAccess.Repositories.GenericRepositories;
using Blogy.Entity.Entities;

namespace Blogy.DataAccess.Repositories.TagRepositories
{
	public class TagRepository : GenericRepository<Tag>, ITagRepository
	{
		public TagRepository(AppDbContext context) : base(context)
		{
		}
	}
}
