using AutoMapper;
using FGVTest.Api.ViewModels.Response;
using FGVTest.Business.DTOs;

namespace FGVTest.Api.Mappings;

/// <summary>
/// Perfil de mapeamento da camada de API (DTO → Response).
/// </summary>
public class ApiMappingProfile : Profile
{
    /// <summary>
    /// Configura os mapeamentos entre DTOs e ViewModels de resposta.
    /// </summary>
    public ApiMappingProfile()
    {
        CreateMap<ClienteDto, ClienteResponse>();
        CreateMap<ProdutoDto, ProdutoResponse>();
        CreateMap<PedidoDto, PedidoResponse>()
            .ForMember(dest => dest.Itens, opt => opt.MapFrom(src => src.Itens));
        CreateMap<ItensPedidoDto, ItemPedidoResponse>();
    }
}
