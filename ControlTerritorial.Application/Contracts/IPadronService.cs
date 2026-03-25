using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlTerritorial.Application.Contracts
{
    public interface IPadronService
    {
        Task<Result<PadronUploadResultDTO>> ImportarPadronAsync(List<PadronRowImportDTO> filas, string fileName);
        Task<Result<PadronActiveDTO?>> ObtenerPadronActivoAsync();
        Task<Result<PadronUploadResultDTO>> SincronizarPadronActivoAsync();
        Task<Result<PadronRowImportDTO?>> ObtenerMatchPorDniAsync(string dni);
    }
}

