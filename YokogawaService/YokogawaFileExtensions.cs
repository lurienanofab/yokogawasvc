/*
   Copyright 2017 University of Michigan

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License. 
*/

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
        public static YokogawaFileData CreateData(this YokogawaFile yokoFile, int lineIndex, DateTime timeStamp, string header, double value)
        {
            if (yokoFile == null) return null;

            return new YokogawaFileData
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

        public static IEnumerable<YokogawaFileData> GetData(this YokogawaFile yokoFile, SampleGranularity gran)
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
                            if (FileUtility.TakeSample(date, gran))
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

        public static IEnumerable<YokogawaFileData> QueryData(this YokogawaFile yokoFile, SampleGranularity gran, DataQueryCriteria criteria)
        {
            var data = GetData(yokoFile, gran);

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
