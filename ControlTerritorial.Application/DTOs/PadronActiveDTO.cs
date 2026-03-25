using System;

namespace ControlTerritorial.Application.DTOs
{
    public class PadronActiveDTO
    {
        public int Id { get; set; }
        public DateTime UploadedAtUtc { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int RowsCount { get; set; }
    }
}

