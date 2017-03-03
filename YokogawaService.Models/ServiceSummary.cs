namespace YokogawaService.Models
{
    public class ServiceSummary
    {
        public long TotalFiles { get; set; }
        public long TotalImports { get; set; }
        public long TotalDataRecords { get; set; }
        public YokogawaFile LastFile { get; set; }
        public FileImport LastImport { get; set; }
        public int CurrentIndex { get; set; }
    }
}
