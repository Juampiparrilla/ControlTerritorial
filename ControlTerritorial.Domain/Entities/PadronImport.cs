using System;
using System.Collections.Generic;

namespace ControlTerritorial.Domain.Entities
{
    public class PadronImport
    {
        public int Id { get; private set; }
        public DateTime UploadedAtUtc { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public List<PadronRow> Rows { get; private set; } = new();

        private PadronImport() { }

        public PadronImport(string fileName, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is required.", nameof(fileName));
            }

            FileName = fileName;
            IsActive = isActive;
            UploadedAtUtc = DateTime.UtcNow;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}

