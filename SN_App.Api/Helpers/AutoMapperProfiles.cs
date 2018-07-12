using System.Linq;
using AutoMapper;
using SN_App.Api.Dtos;
using SN_App.Repo.Models;

namespace SN_App.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForDetailDto>()
                .ForMember(
                    destinationMember => destinationMember.PhotoUrl,
                    option => option.MapFrom(
                        u => u.Photos.FirstOrDefault(
                            p => p.IsMain
                        ).Url
                    )
                )
                .ForMember(
                    destinationMember => destinationMember.Age,
                    option => option.ResolveUsing(
                        a => a.DateOfBirth.CalculateAge()
                    )
                );

            CreateMap<User, UserForListDto>()
                .ForMember(
                        destinationMember => destinationMember.PhotoUrl,
                        option => option.MapFrom(
                            u => u.Photos.FirstOrDefault(
                                p => p.IsMain
                            ).Url
                        )
                    )
                .ForMember(
                    destinationMember => destinationMember.Age,
                    option => option.ResolveUsing(
                        a => a.DateOfBirth.CalculateAge()
                    )
                );
                
            CreateMap<Photo, PhotoForDetailedDto>();
        }
    }
}