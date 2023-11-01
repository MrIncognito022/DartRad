using AutoMapper;
using DartRad.Areas.ContentCreator.Models;

namespace DartRad.MappingProfiles
{
    public class AnswerHotspotProfile : Profile
    {
        public AnswerHotspotProfile()
        {
            CreateMap<CreateHotspotAnswerViewModel, AnswerHotspot>();
            CreateMap<AnswerHotspot, AnswerHotspotViewModel>();
        }
    }
}
