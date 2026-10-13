using AutoMapper;
using TerraPulse.Api.Models;
using TerraPulse.Api.ViewModels.AlertasAmbientais;
using TerraPulse.Api.ViewModels.AreasMonitoradas;
using TerraPulse.Api.ViewModels.IndicadoresAmbientais;
using TerraPulse.Api.ViewModels.RecomendacoesSustentaveis;

namespace TerraPulse.Api.Mappings;

public class TerraPulseMappingProfile : Profile
{
    public TerraPulseMappingProfile()
    {
        CreateMap<AreaMonitorada, AreaMonitoradaViewModel>()
            .ForMember(dest => dest.TipoAreaDescricao,
                opt => opt.MapFrom(src => src.TipoArea.ToString()))
            .ForMember(dest => dest.StatusDescricao,
                opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<AreaMonitoradaCreateViewModel, AreaMonitorada>();

        CreateMap<AreaMonitoradaUpdateViewModel, AreaMonitorada>();

        CreateMap<IndicadorAmbiental, IndicadorAmbientalViewModel>()
            .ForMember(dest => dest.TipoIndicadorDescricao,
        opt => opt.MapFrom(src => src.TipoIndicador.ToString()))
            .ForMember(dest => dest.AreaMonitoradaNome,
        opt => opt.MapFrom(src => src.AreaMonitorada != null ? src.AreaMonitorada.Nome : null));

        CreateMap<IndicadorAmbientalCreateViewModel, IndicadorAmbiental>();

        CreateMap<AlertaAmbiental, AlertaAmbientalViewModel>()
            .ForMember(dest => dest.TipoAlertaDescricao,
        opt => opt.MapFrom(src => src.TipoAlerta.ToString()))
            .ForMember(dest => dest.SeveridadeDescricao,
        opt => opt.MapFrom(src => src.Severidade.ToString()))
            .ForMember(dest => dest.StatusDescricao,
        opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.AreaMonitoradaNome,
        opt => opt.MapFrom(src => src.AreaMonitorada != null ? src.AreaMonitorada.Nome : null));

        CreateMap<RecomendacaoSustentavel, RecomendacaoSustentavelViewModel>()
            .ForMember(dest => dest.StatusDescricao,
        opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.AlertaTitulo,
        opt => opt.MapFrom(src => src.AlertaAmbiental != null ? src.AlertaAmbiental.Titulo : null))
            .ForMember(dest => dest.AreaMonitoradaNome,
        opt => opt.MapFrom(src =>
            src.AlertaAmbiental != null && src.AlertaAmbiental.AreaMonitorada != null
                ? src.AlertaAmbiental.AreaMonitorada.Nome
                : null));

    }
}