using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlTerritorial.Infrastructure.Implementations
{
    public class PadronService : IPadronService
    {
        private readonly DbContextControlTerritorial _context;

        public PadronService(DbContextControlTerritorial context)
        {
            _context = context;
        }

        public async Task<Result<PadronUploadResultDTO>> ImportarPadronAsync(List<PadronRowImportDTO> filas, string fileName)
        {
            if (filas == null || filas.Count == 0)
            {
                return Result<PadronUploadResultDTO>.Fail("El padrón no tiene filas válidas.");
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            // Asegurar que PadronFilas contenga solo el último padrón importado.
            // Requerimiento: si el usuario carga un nuevo padrón, NO deben quedar filas viejas.
            var importsActivos = await _context.PadronesImportados.Where(p => p.IsActive).ToListAsync();
            foreach (var a in importsActivos)
            {
                a.SetActive(false);
            }
            await _context.SaveChangesAsync();

            var filasExistentes = await _context.PadronFilas.ToListAsync();
            if (filasExistentes.Count > 0)
            {
                _context.PadronFilas.RemoveRange(filasExistentes);
                await _context.SaveChangesAsync();
            }

            var import = new PadronImport(fileName, true);
            _context.PadronesImportados.Add(import);
            await _context.SaveChangesAsync();

            // Guardar filas.
            var rows = filas.Select(f =>
                    new PadronRow(f.DNI, f.Nombre, f.Apellido, f.EscuelaNombre, f.MesaNro, f.Orden, import.Id))
                .ToList();

            _context.PadronFilas.AddRange(rows);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            // No sincronizamos todavía: lo hace el endpoint "sync-active" (2 pasos en UI).
            return Result<PadronUploadResultDTO>.Success(new PadronUploadResultDTO
            {
                TotalRows = rows.Count,
                RowsStored = rows.Count,
                PersonasUpdated = 0,
                DnisNotFound = new List<string>()
            });
        }

        public async Task<Result<PadronActiveDTO?>> ObtenerPadronActivoAsync()
        {
            var active = await _context.PadronesImportados
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IsActive);

            if (active == null)
            {
                return Result<PadronActiveDTO?>.Success(null);
            }

            var rowsCount = await _context.PadronFilas.CountAsync(r => r.PadronImportId == active.Id);

            return Result<PadronActiveDTO?>.Success(new PadronActiveDTO
            {
                Id = active.Id,
                UploadedAtUtc = active.UploadedAtUtc,
                FileName = active.FileName,
                RowsCount = rowsCount
            });
        }

        public async Task<Result<PadronRowImportDTO?>> ObtenerMatchPorDniAsync(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return Result<PadronRowImportDTO?>.Fail("DNI requerido.");
            }

            var normalized = dni.Trim();

            var active = await _context.PadronesImportados.FirstOrDefaultAsync(p => p.IsActive);
            if (active == null)
            {
                return Result<PadronRowImportDTO?>.Success(null);
            }

            var row = await _context.PadronFilas
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PadronImportId == active.Id && r.DNI == normalized);

            if (row == null)
            {
                return Result<PadronRowImportDTO?>.Success(null);
            }

            return Result<PadronRowImportDTO?>.Success(new PadronRowImportDTO
            {
                DNI = row.DNI,
                Nombre = row.Nombre,
                Apellido = row.Apellido,
                EscuelaNombre = row.EscuelaNombre,
                MesaNro = row.MesaNro,
                Orden = row.Orden
            });
        }

        public async Task<Result<PadronUploadResultDTO>> SincronizarPadronActivoAsync()
        {
            var active = await _context.PadronesImportados.FirstOrDefaultAsync(p => p.IsActive);
            if (active == null)
            {
                return Result<PadronUploadResultDTO>.Fail("No hay padrón activo para sincronizar.");
            }

            var rows = await _context.PadronFilas
                .Where(r => r.PadronImportId == active.Id)
                .ToListAsync();

            var syncResult = await SyncRowsAsync(rows);
            return Result<PadronUploadResultDTO>.Success(syncResult);
        }

        private async Task<PadronUploadResultDTO> SyncRowsAsync(List<PadronRow> rows)
        {
            var dniSet = rows.Select(r => r.DNI).Distinct().ToHashSet(StringComparer.Ordinal);
            var escSet = rows.Select(r => r.EscuelaNombre).Distinct().ToHashSet(StringComparer.Ordinal);

            var personas = await _context.Personas.Where(p => dniSet.Contains(p.DNI)).ToListAsync();
            var personasByDni = personas.ToDictionary(p => p.DNI, p => p, StringComparer.Ordinal);

            // Escuelas: cargar existentes.
            var escuelasExistentes = await _context.Escuelas
                .Where(e => escSet.Contains(e.NombreEstablecimiento!))
                .ToListAsync();
            var escuelaByNombre = escuelasExistentes.ToDictionary(e => e.NombreEstablecimiento!, e => e, StringComparer.Ordinal);

            var escuelasParaCrear = escSet.Where(n => !escuelaByNombre.ContainsKey(n)).ToList();
            foreach (var nombre in escuelasParaCrear)
            {
                escuelaByNombre[nombre] = new Escuela(nombre, string.Empty);
                _context.Escuelas.Add(escuelaByNombre[nombre]);
            }
            if (escuelasParaCrear.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            // Mesas: cargar existentes.
            var mesaKeys = rows
                .Select(r => new { EscuelaNombre = r.EscuelaNombre, r.MesaNro })
                .Distinct()
                .ToList();

            var escuelaIds = mesaKeys.Select(k => escuelaByNombre[k.EscuelaNombre].Id).ToHashSet();
            var nroMesaSet = mesaKeys.Select(k => k.MesaNro).ToHashSet();

            var mesasExistentes = await _context.Mesas
                .Where(m => escuelaIds.Contains(m.EscuelaId) && nroMesaSet.Contains(m.NroMesa))
                .ToListAsync();

            var mesaByKey = mesasExistentes.ToDictionary(
                m => $"{m.EscuelaId}|{m.NroMesa}",
                m => m,
                StringComparer.Ordinal);

            foreach (var key in mesaKeys)
            {
                var escuelaId = escuelaByNombre[key.EscuelaNombre].Id;
                var mesaKey = $"{escuelaId}|{key.MesaNro}";
                if (!mesaByKey.ContainsKey(mesaKey))
                {
                    // Buscar la primera fila que matchee para traer el Orden.
                    var orden = rows.First(r => r.EscuelaNombre == key.EscuelaNombre && r.MesaNro == key.MesaNro).Orden;
                    var nuevaMesa = new Mesa(key.MesaNro, orden);
                    nuevaMesa.AsignarEscuela(escuelaByNombre[key.EscuelaNombre]);
                    _context.Mesas.Add(nuevaMesa);
                    mesaByKey[mesaKey] = nuevaMesa;
                }
            }

            // Si existía mesa pero con orden distinto, actualizaremos.
            foreach (var r in rows)
            {
                var escuelaId = escuelaByNombre[r.EscuelaNombre].Id;
                var mesaKey = $"{escuelaId}|{r.MesaNro}";
                var mesa = mesaByKey[mesaKey];
                if (mesa != null)
                {
                    // Asegurar el orden del padrón.
                    mesa.ActualizarOrden(r.Orden);
                }
            }

            await _context.SaveChangesAsync();

            var personasActualizadas = 0;
            var dnisNotFound = new List<string>();

            foreach (var row in rows)
            {
                if (!personasByDni.TryGetValue(row.DNI, out var persona))
                {
                    // evitar duplicados en lista
                    if (!dnisNotFound.Contains(row.DNI)) dnisNotFound.Add(row.DNI);
                    continue;
                }

                var escuela = escuelaByNombre[row.EscuelaNombre];
                var escuelaId = escuela.Id;
                var mesaKey = $"{escuelaId}|{row.MesaNro}";
                var mesa = mesaByKey[mesaKey];
                persona.CompletarCamposDesdePadron(row.Nombre, row.Apellido, escuela, mesa);
                personasActualizadas++;
            }

            await _context.SaveChangesAsync();

            return new PadronUploadResultDTO
            {
                TotalRows = rows.Count,
                RowsStored = rows.Count,
                PersonasUpdated = personasActualizadas,
                DnisNotFound = dnisNotFound
            };
        }
    }
}

