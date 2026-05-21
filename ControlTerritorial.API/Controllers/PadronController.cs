using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;

namespace ControlTerritorial.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PadronController : ControllerBase
    {
        private readonly IPadronService _padronService;

        public PadronController(IPadronService padronService)
        {
            _padronService = padronService;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(30_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPadron(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Debes enviar un archivo Excel.");
            }

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("El archivo debe ser .xlsx");
            }

            var filas = new List<PadronRowImportDTO>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var ws = workbook.Worksheets.First();

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
                // Formato: DNI, nombre, apellido, Escuela, mesa, Orden (6 columnas)
                for (var r = 2; r <= lastRow; r++)
                {
                    var dni = ws.Row(r).Cell(1).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(dni)) continue;

                    var nombre = ws.Row(r).Cell(2).GetString().Trim();
                    var apellido = ws.Row(r).Cell(3).GetString().Trim();
                    var escuela = ws.Row(r).Cell(4).GetString().Trim();
                    var mesaStr = ws.Row(r).Cell(5).GetString().Trim();
                    var orden = ws.Row(r).Cell(6).GetString().Trim();

                    if (!int.TryParse(mesaStr, out var mesaNro))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(escuela))
                    {
                        continue;
                    }

                    // Orden puede venir vacío.
                    filas.Add(new PadronRowImportDTO
                    {
                        DNI = dni,
                        Nombre = nombre,
                        Apellido = apellido,
                        EscuelaNombre = escuela,
                        MesaNro = mesaNro,
                        Orden = string.IsNullOrWhiteSpace(orden) ? null : orden
                    });
                }
            }

            var result = await _padronService.ImportarPadronAsync(filas, file.FileName);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Value);
        }

        [HttpPost("sync-active")]
        public async Task<IActionResult> SyncActivePadron()
        {
            var result = await _padronService.SincronizarPadronActivoAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Value);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetPadronActivo()
        {
            var result = await _padronService.ObtenerPadronActivoAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Value);
        }

        [HttpGet("match-by-dni/{dni}")]
        public async Task<IActionResult> MatchByDni(string dni)
        {
            var result = await _padronService.ObtenerMatchPorDniAsync(dni);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            if (result.Value == null) return NotFound();

            return Ok(result.Value);
        }
    }
}

