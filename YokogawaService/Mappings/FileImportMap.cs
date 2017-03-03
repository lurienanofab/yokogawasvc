using FluentNHibernate.Mapping;
using YokogawaService.Models;

namespace YokogawaService.Mappings
{
    internal class FileImportMap: ClassMap<FileImport>
    {
        internal FileImportMap()
        {
            Schema("Meter.dbo");
            Table("FileImport");
            Id(x => x.FileImportID);
            Map(x => x.FileIndex);
            Map(x => x.FilePath);
            Map(x => x.ImportDate);
            Map(x => x.LineCount);
        }
    }
}
