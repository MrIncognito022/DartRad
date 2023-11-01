using AutoMapper;
using DartRad.Areas.Editor.Models;
using DartRad.Areas.ContentCreator.Models;

namespace DartRad.MappingProfiles
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<Quiz, QuizListViewModel>()
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToFriendlyString()))
              .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedByAdmin.Name));

            CreateMap<Quiz, QuizUpdateViewModel>().ReverseMap();

            CreateMap<Quiz, QuizDetailsViewModel>()
                 .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedByAdmin.Name));

            CreateMap<Quiz, PendingQuizListViewModel>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.ContentCreator.Name));
            
            CreateMap<Quiz, PendingQuizDetailsViewModel>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.ContentCreator.Name))
                .ForMember(dest => dest.QuizNotes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<Quiz, PublicQuizViewModel>();

            CreateMap<Quiz, PublicQuizListViewModel>()
                .ForMember(x => x.CreatedBy, opt => opt.MapFrom(y => y.ContentCreator.Name))
                .ForMember(x => x.CreatedAt, opt => opt.MapFrom(y => y.CreatedAt.ToString("MMM yyyy")));

        }
    }
}
