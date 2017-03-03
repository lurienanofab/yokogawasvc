using System;

namespace YokogawaService.Models
{
    public class FileImport
    {
        public virtual int FileImportID { get; set; }

        public virtual int FileIndex { get; set; }

        public virtual string FilePath { get; set; }

        public virtual DateTime ImportDate { get; set; }

        public virtual int LineCount { get; set; }
    }
}
