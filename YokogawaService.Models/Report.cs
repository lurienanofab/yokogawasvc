namespace YokogawaService.Models
{
    public class Report
    {
        public virtual int ReportID { get; set; }
        public virtual string ReportType { get; set; }
        public virtual string ReportName { get; set; }
        public virtual string Header { get; set; }
        public virtual double UnitCost { get; set; }
        public virtual string BorderColor { get; set; }
        public virtual string BackgroundColor { get; set; }
        public virtual string PointBorderColor { get; set; }
        public virtual string PointBackgroundColor { get; set; }
        public virtual bool Active { get; set; }
    }
}
