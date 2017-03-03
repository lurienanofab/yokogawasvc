using System;

namespace YokogawaService.Models
{
    public class MeterData
    {
        public virtual int MeterDataID { get; set; }

        public virtual int FileIndex { get; set; }

        public virtual int LineIndex { get; set; }

        public virtual string Header { get; set; }

        public virtual DateTime TimeStamp { get; set; }

        public virtual double Value { get; set; }
    }
}
