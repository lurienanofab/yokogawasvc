using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YokogawaService.Models;

namespace YokogawaService
{
    public static class YokogawaFileExtensions
    {
        public static MeterData CreateData(this YokogawaFile yokoFile, int lineIndex, DateTime timeStamp, string header, double value)
        {
            if (yokoFile == null) return null;

            return new MeterData
            {
                 
                FileIndex = yokoFile.Index,
                LineIndex = lineIndex,
                TimeStamp = timeStamp,
                Header = header,
                Value = value
            };
        }

        public static string GetContent(this YokogawaFile yokoFile)
        {
            if (yokoFile == null) return null;
            return File.ReadAllText(yokoFile.FilePath);
        }

        public static IEnumerable<MeterData> GetData(this YokogawaFile yokoFile)
        {
            if (yokoFile != null)
            {
                string[] headers = null;
                int lineIndex = 1;

                foreach (string line in File.ReadAllLines(yokoFile.FilePath))
                {
                    if (line.StartsWith("Time Stamp,"))
                    {
                        headers = line.Split(',');
                    }
                    else if (headers != null && line.Length > 23)
                    {
                        string datestr = line.Substring(0, 23);
                        DateTime date = DateTime.MinValue;

                        if (DateTime.TryParse(datestr, out date))
                        {
                            //we only take the sample at midnight, otherwise there are way too many rows
                            if (FileUtility.TakeSample(date))
                            {
                                string[] items = line.Split(',');
                                if (items.Length == headers.Length)
                                {
                                    //start at 1 to skip the timestamp column
                                    for (int i = 1; i < headers.Length; i++)
                                    {
                                        //example: we only care about headers than end with "[Liters]- Max"
                                        //  HeaderPattern: \[Liters\] - Max$
                                        if (Regex.IsMatch(headers[i], Config.Current.HeaderPattern))
                                        {
                                            yield return yokoFile.CreateData(lineIndex, date, headers[i], double.Parse(items[i]));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    lineIndex += 1;
                }
            }
        }

        public static IEnumerable<MeterData> QueryData(this YokogawaFile yokoFile, DataQueryCriteria criteria)
        {
            var data = GetData(yokoFile);

            if (data == null) return null;

            if (!string.IsNullOrEmpty(criteria.HeaderPattern))
                data = data.Where(x => Regex.IsMatch(x.Header, criteria.HeaderPattern));

            if (criteria.StartDate.HasValue)
                data = data.Where(x => x.TimeStamp >= criteria.StartDate.Value);

            if (criteria.EndDate.HasValue)
                data = data.Where(x => x.TimeStamp < criteria.EndDate.Value);

            return data;
        }
    }
}
