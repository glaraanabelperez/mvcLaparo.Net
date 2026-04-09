using AutoMapper;
using Models;
using mvc.Laparoscopy.Models;
using QueryService.Models;
using Utils;

namespace mvc.Laparoscopy.Mapper
{
    public class AutomapperToViewModel : Profile
    {
        public AutomapperToViewModel()
        {
            //-- Users  -----------

            //CreateMap<UserCreateCommand, Models.User>().ReverseMap();
            //CreateMap<DataCollection<UserCreateCommand>, DataCollection<Models.User>>().ForMember(dest => dest.Items, sour => sour.MapFrom(s => s.Items));
            //CreateMap<UserUpdateCommand, Models.User>();

            //-- Profile  -----------
            CreateMap<ProductViewModel,ProductDto>().ReverseMap();
            CreateMap(typeof(DataCollection<>), typeof(PagedResponse<>))
                .ForMember("Items", opt => opt.MapFrom("Items"))
                .ForMember("Total", opt => opt.MapFrom("Total"))
                .ForMember("Page", opt => opt.MapFrom("Page"))
                .ForMember("Pages", opt => opt.MapFrom("Pages"))
                .ForMember("HasItems", opt => opt.MapFrom("HasItems"));
        }

    }

}