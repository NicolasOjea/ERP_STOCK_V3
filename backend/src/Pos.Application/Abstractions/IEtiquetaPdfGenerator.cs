using Pos.Application.DTOs.Etiquetas;

namespace Pos.Application.Abstractions;

public interface IEtiquetaPdfGenerator
{
    byte[] Generate(EtiquetaPdfDataDto data);
}
