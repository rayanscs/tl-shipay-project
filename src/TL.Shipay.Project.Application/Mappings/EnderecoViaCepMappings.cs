using AutoMapper;
using TL.Shipay.Project.Domain.Models;
using TL.Shipay.Project.Domain.Models.Responses.ViaCep;

namespace TL.Shipay.Project.Application.Mappings
{
    public class EnderecoViaCepMappings : Profile
    {
        public EnderecoViaCepMappings()
           => CreateMap<ViaCepResponse, Endereco>()
                   .ForMember(dest => dest.Cep, opt => opt.MapFrom(src => src.Cep))
                   .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Logradouro))
                   .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Localidade))
                   .ReverseMap();
    }
}
