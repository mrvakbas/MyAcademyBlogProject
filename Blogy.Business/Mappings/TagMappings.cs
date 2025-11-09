using AutoMapper;
using Blogy.Business.DTOs.TagDtos;
using Blogy.Entity.Entities;

namespace Blogy.Business.Mappings
{
	public class TagMappings : Profile
	{
		public TagMappings()
		{
			CreateMap<Tag, ResultTagDto>().ReverseMap();
			CreateMap<Tag, UpdateTagDto>().ReverseMap();
			CreateMap<Tag, CreateTagDto>().ReverseMap();
		}
	}
}
