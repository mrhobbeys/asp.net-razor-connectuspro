using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SiteBlue.Core.DocumentGeneration
{
    public class DocumentResult : IDisposable
    {
        public bool Success { get; set; }
        public Stream Data { get; set; }
        public string Message { get; set; }

        public void Dispose()
        {
            if (Data != null)
                Data.Dispose();
        }
    }
}
