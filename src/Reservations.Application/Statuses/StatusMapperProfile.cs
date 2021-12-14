using AutoMapper;
using Reservations.Domain.StatusAggregate;

namespace Reservations.Application.Statuses
{
    public class StatusMapperProfile : Profile
    {
        public StatusMapperProfile()
        {
            CreateMap<Status, StatusDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
                ;
        }
    }
}
