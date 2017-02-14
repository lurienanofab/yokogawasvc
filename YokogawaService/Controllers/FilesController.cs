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
using System.Linq;
using System.Web.Http;
using YokogawaService.Models;

namespace YokogawaService.Controllers
{
    public class FilesController : ApiController
    {
        [Route("files")]
        public IEnumerable<YokogawaFile> GetFiles(int skip = 0, int take = 100)
        {
            return FileUtility.GetAllFiles(skip, take);
        }

        [Route("files/{index}")]
        public YokogawaFile GetFile(int index)
        {
            return FileUtility.GetFile(index);
        }

        [Route("files/{index}/content")]
        public string GetFileContent(int index)
        {
            YokogawaFile yokoFile = FileUtility.GetFile(index);
            return yokoFile.GetContent();
        }

        [Route("files/{index}/data")]
        public IEnumerable<YokogawaFileData> GetFileData(int index, string gran = "day")
        {
            YokogawaFile yokoFile = FileUtility.GetFile(index);
            return yokoFile.GetData(FileUtility.GetGranularity(gran));
        }

        [HttpPost, Route("files/{index}/data")]
        public IEnumerable<YokogawaFileData> QueryFileData([FromBody] DataQueryCriteria criteria, int index, string gran = "day")
        {
            YokogawaFile yokoFile = FileUtility.GetFile(index);
            return yokoFile.QueryData(FileUtility.GetGranularity(gran), criteria);
        }

        [HttpPost, Route("files/{index}/import")]
        public ImportFileModel ImportFile(int index, string gran = "day")
        {
            YokogawaFile yokoFile;

            if (index == -1)
                yokoFile = FileUtility.GetFirstFile();
            else
                yokoFile = FileUtility.GetFile(index);

            if (yokoFile == null) return null;

            ImportFile fileImport = ImportManager.Current.GetImportFile(index);

            if (fileImport == null)
                fileImport = ImportManager.Current.ImportFile(yokoFile, FileUtility.GetGranularity(gran));

            return ModelFactory.CreateImportFileModel(fileImport);
        }

        [HttpGet, Route("files/next")]
        public YokogawaFile GetNextFile()
        {
            int index = GetNextIndex();
            return GetFile(index);
        }

        [HttpGet, Route("files/next/content")]
        public string GetNextFileContent()
        {
            int index = GetNextIndex();
            return GetFileContent(index);
        }

        [HttpGet, Route("files/next/data")]
        public IEnumerable<YokogawaFileData> GetNextFileData(string gran = "day")
        {
            int index = GetNextIndex();
            return GetFileData(index, gran);
        }

        [HttpPost, Route("files/next/data")]
        public IEnumerable<YokogawaFileData> QueryNextFileData([FromBody] DataQueryCriteria criteria, string gran = "day")
        {
            int index = GetNextIndex();
            return QueryFileData(criteria, index, gran);
        }

        [HttpPost, Route("files/next/import")]
        public ImportFileModel ImportNextFile(string gran = "day")
        {
            var lastIndex = ImportManager.Current.GetIndex();
            int nextIndex = GetNextIndex(lastIndex);

            var result = ImportFile(nextIndex, gran);

            if (result != null)
                ImportManager.Current.SetIndex(lastIndex, result.Index);

            return result;
        }

        [Route("files/next/index")]
        public int GetNextIndex()
        {
            var lastIndex = ImportManager.Current.GetIndex();
            return GetNextIndex(lastIndex);
        }

        private int GetNextIndex(ImportFileIndex lastIndex)
        {
            if (lastIndex == null)
            {
                var firstFile = FileUtility.GetFirstFile();
                if (firstFile == null)
                    return -1;
                else
                    return firstFile.Index;
            }
            else
                return lastIndex.Index + 1;
        }
    }
}
