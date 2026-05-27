using AutoMapper;
using FGVTest.Business.DTOs;
using FGVTest.Business.Entities;

namespace FGVTest.Business.Mappings;

/// <summary>
/// Perfil de mapeamento da camada de negócio (Entidade → DTO).
/// </summary>
public class BusinessMappingProfile : Profile
{
    /// <summary>
    /// Configura os mapeamentos entre entidades e DTOs.
    /// </summary>
    public BusinessMappingProfile()
    {
        CreateMap<Cliente, ClienteDto>().ReverseMap();
        CreateMap<Produto, ProdutoDto>().ReverseMap();
        CreateMap<Pedido, PedidoDto>().ReverseMap();
        CreateMap<ItensPedido, ItensPedidoDto>().ReverseMap();
    }
}
