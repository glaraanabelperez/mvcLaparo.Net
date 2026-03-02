using AutoMapper;
using Models;
using QueryService.Models;
using Utils;

namespace QueryService.Mapper
{
    public class AutomapperToDto : Profile
    {
        public AutomapperToDto()
        {
            //-- Users  -----------

            //CreateMap<UserCreateCommand, Models.User>().ReverseMap();
            //CreateMap<DataCollection<UserCreateCommand>, DataCollection<Models.User>>().ForMember(dest => dest.Items, sour => sour.MapFrom(s => s.Items));
            //CreateMap<UserUpdateCommand, Models.User>();

            //-- Profile  -----------
            CreateMap<Product,ProductDto>().ReverseMap();
            CreateMap<DataCollection<Product>, DataCollection<ProductDto>>().ForMember(dest => dest.Items, sour => sour.MapFrom(s => s.Items));

            CreateMap<Discount, DiscountDto>().ReverseMap();

        }

    }

}