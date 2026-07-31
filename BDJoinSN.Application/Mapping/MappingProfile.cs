

using AutoMapper;
using BDJoinSN.Application.Features.Posts.Commands.CreatePost;
using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;
using BDJoinSN.Application.Features.Users.Commands.UpdateProfilesCommand;
using BDJoinSN.Domain;

namespace BDJoinSN.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<CreatePostCommand, Post>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Author));

            CreateMap<Post, PostDto>();

            CreateMap<UpdateUserProfileCommand, UserProfile>()
            .ForMember(dest => dest.Name, opt =>
            {
                opt.Condition(src => src.Name != null);
                opt.MapFrom(src => src.Name);
            })
            .ForMember(dest => dest.LastName, opt =>
            {
                opt.Condition(src => src.LastName != null);
                opt.MapFrom(src => src.LastName);
            })
            .ForMember(dest => dest.Biography, opt =>
            {
                opt.Condition(src => src.Biography != null);
                opt.MapFrom(src => src.Biography);
            })
            .ForMember(dest => dest.Location, opt =>
            {
                opt.Condition(src => src.Location != null);
                opt.MapFrom(src => src.Location);
            })
            .ForMember(dest => dest.Birthday, opt =>
            {
                opt.Condition(src => src.Birthday != null);
                opt.MapFrom(src => src.Birthday);
            });
        }
    }
}
