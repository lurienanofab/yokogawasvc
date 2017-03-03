using FluentNHibernate.Mapping;
using YokogawaService.Models;

namespace YokogawaService.Mappings
{
    internal class MeterDataMap : ClassMap<MeterData>
    {
        internal MeterDataMap()
        {
            Schema("Meter.dbo");
            Table("MeterData");
            Id(x => x.MeterDataID);
            Map(x => x.FileIndex);
            Map(x => x.LineIndex);
            Map(x => x.Header);
            Map(x => x.TimeStamp);
            Map(x => x.Value);
        }
    }
}
