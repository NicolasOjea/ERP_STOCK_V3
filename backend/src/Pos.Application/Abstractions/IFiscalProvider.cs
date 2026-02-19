using Pos.Application.DTOs.Comprobantes;

namespace Pos.Application.Abstractions;

public interface IFiscalProvider
{
    Task<FiscalEmitResultDto> EmitirAsync(
        FiscalEmitRequestDto request,
        CancellationToken cancellationToken = default);
}
