using Pos.Application.DTOs.Stock;

namespace Pos.Application.Abstractions;

public interface IRemitoPdfGenerator
{
    byte[] Generate(StockRemitoPdfDataDto data);
}
