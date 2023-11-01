using AutoMapper;
using DartRad.Areas.Editor.Models;

namespace DartRad.MappingProfiles
{
    public class QuizNoteProfile : Profile
    {
        public QuizNoteProfile()
        {
            CreateMap<QuizNote, QuizNoteViewModel>()
                .ForMember(x => x.AdminName, opt => opt.MapFrom(src => src.Editor.Name));
        }
    }
}
