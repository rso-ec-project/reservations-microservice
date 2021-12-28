using AutoMapper;
using Reservations.Domain.ReservationAggregate;

namespace Reservations.Application.Reservations
{
    public class ReservationMapperProfile : Profile
    {
        public ReservationMapperProfile()
        {
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.From, opts => opts.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opts => opts.MapFrom(src => src.To))
                .ForMember(dest => dest.Code, opts => opts.MapFrom(src => src.Code))
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ChargerId, opts => opts.MapFrom(src => src.ChargerId))
                .ForMember(dest => dest.StatusId, opts => opts.MapFrom(src => src.StatusId))
                ;

            CreateMap<ReservationPostDto, Reservation>()
                .ForMember(dest => dest.From, opts => opts.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opts => opts.MapFrom(src => src.To))
                .ForMember(dest => dest.Code, opts => opts.MapFrom(src => ""))
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ChargerId, opts => opts.MapFrom(src => src.ChargerId))
                .ForMember(dest => dest.StatusId, opts => opts.MapFrom(src => 1))
                ;
        }
    }
}
