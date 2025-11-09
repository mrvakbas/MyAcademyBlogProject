using Blogy.Business.DTOs.CommentDtos;

namespace Blogy.Business.Services.CommentService
{
	public interface ICommentService : IGenericService<ResultCommentDto, UpdateCommentDto, CreateCommentDto>
	{
	}
}
