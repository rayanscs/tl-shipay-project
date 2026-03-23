using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using TL.Shipay.Project.Domain.Models;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;

namespace TL.Shipay.Project.Application.Mappings
{
    [ExcludeFromCodeCoverage]
    public class DadosEmpresaMappings : Profile
    {
        public DadosEmpresaMappings()
            => CreateMap<DadosCnpjBrasilApiResponse, DadosEmpresa>()
                //.ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Logradouro))
                //.ForMember(dest => dest.Municipio, opt => opt.MapFrom(src => src.Municipio))
                //.ForMember(dest => dest.Cep, opt => opt.MapFrom(src => src.Cep))
                //.ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.Cnpj))
                //.ForMember(dest => dest.RazaoSocial, opt => opt.MapFrom(src => src.RazaoSocial))
                //.ForMember(dest => dest.NomeFantasia, opt => opt.MapFrom(src => src.NomeFantasia))
                .ReverseMap();
    }
}
