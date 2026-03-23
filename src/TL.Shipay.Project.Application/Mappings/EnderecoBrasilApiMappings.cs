using AutoMapper;
using TL.Shipay.Project.Domain.Models;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCep;

namespace TL.Shipay.Project.Application.Mappings
{
    public class EnderecoBrasilApiMappings : Profile
    {
        public EnderecoBrasilApiMappings()
            => CreateMap<BrasilApiCepResponse, Endereco>()
                    .ForMember(dest => dest.Cep, opt => opt.MapFrom(src => src.Cep))
                    .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Street))
                    .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.City))
                    .ReverseMap();        
    }
}
