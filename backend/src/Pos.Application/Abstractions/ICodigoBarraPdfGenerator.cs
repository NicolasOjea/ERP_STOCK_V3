using Pos.Application.DTOs.Etiquetas;

namespace Pos.Application.Abstractions;

public interface ICodigoBarraPdfGenerator
{
    byte[] Generate(CodigoBarraPdfDataDto data);
}
