using System.Collections.Generic;

namespace ControlTerritorial.Application.DTOs
{
    public class PadronUploadResultDTO
    {
        public int TotalRows { get; set; }
        public int RowsStored { get; set; }
        public int PersonasUpdated { get; set; }
        public List<string> DnisNotFound { get; set; } = new();
    }
}

