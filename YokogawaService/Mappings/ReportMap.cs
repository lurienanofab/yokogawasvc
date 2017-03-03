using FluentNHibernate.Mapping;
using YokogawaService.Models;

namespace YokogawaService.Mappings
{
    internal class ReportMap : ClassMap<Report>
    {
        internal ReportMap()
        {
            Schema("Meter.dbo");
            Table("Report");
            Id(x => x.ReportID);
            Map(x => x.ReportType);
            Map(x => x.ReportName);
            Map(x => x.Header);
            Map(x => x.UnitCost);
            Map(x => x.BorderColor);
            Map(x => x.BackgroundColor);
            Map(x => x.PointBorderColor);
            Map(x => x.PointBackgroundColor);
            Map(x => x.Active);
        }
    }
}
