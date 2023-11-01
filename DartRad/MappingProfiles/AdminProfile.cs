using AutoMapper;
using DartRad.Areas.SuperAdmin.Models;

namespace DartRad.MappingProfiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<Editor, AdminListViewModel>()
                .ForMember(x => x.CreatedBySuperAdmin , opt => opt.MapFrom(src => src.CreatedByUser.Name))
                .ForMember(x => x.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("MMMM yyyy")));

            CreateMap<AdminCreateViewModel, Editor>();
            CreateMap<Editor, AdminUpdateViewModel>();
            CreateMap<AdminUpdateViewModel, Editor>();
        }
    }
}
