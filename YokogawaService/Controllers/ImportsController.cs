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
    public class ImportsController : ApiController
    {
        [Route("imports")]
        public IEnumerable<ImportFileModel> GetImports(int skip = 0, int take = 100)
        {
            return ImportManager.Current.QueryImportFiles().Skip(skip).Take(take).Select(ModelFactory.CreateImportFileModel);
        }

        [Route("imports/{index}")]
        public ImportFileModel GetFileImport(int index)
        {
            ImportFile fileImport = ImportManager.Current.GetImportFile(index);
            return ModelFactory.CreateImportFileModel(fileImport);
        }

        [Route("imports/{index}")]
        public bool DeleteFileImport(int index)
        {
            return ImportManager.Current.DeleteFileImport(index);
        }

        [Route("imports/{index}/data")]
        public IEnumerable<YokogawaFileData> GetFileImportData(int index)
        {
            var query = ImportManager.Current.QueryImportFileData().Where(x => x.FileIndex == index);
            return query.Select(ModelFactory.CreateYokogawaFileData);
        }

        [Route("imports/data")]
        public IEnumerable<YokogawaFileData> GetData(int skip = 0, int take = 100)
        {
            return ImportManager.Current.QueryImportFileData().Skip(skip).Take(take).Select(ModelFactory.CreateYokogawaFileData);
        }

        [HttpPost, Route("imports/data")]
        public IEnumerable<YokogawaFileData> QueryFileImportData([FromBody] DataQueryCriteria criteria)
        {
            var query = ImportManager.Current.QueryImportFileData(criteria);
            return query.Select(ModelFactory.CreateYokogawaFileData);
        }

        [Route("imports/last")]
        public ImportFileModel GetLastImport()
        {
            var importIndex = ImportManager.Current.GetIndex();

            if (importIndex != null)
            {
                ImportFile importFile = ImportManager.Current.GetImportFile(importIndex.Index);
                return ModelFactory.CreateImportFileModel(importFile);
            }

            return null;
        }

        [Route("imports/last/data")]
        public IEnumerable<YokogawaFileData> GetLastImportData()
        {
            var importIndex = ImportManager.Current.GetIndex();

            if (importIndex != null)
            {
                return GetFileImportData(importIndex.Index);
            }

            return null;
        }

        [Route("imports/index")]
        public int GetIndex()
        {
            var importIndex = ImportManager.Current.GetIndex();

            if (importIndex == null)
                return -1;
            else
                return importIndex.Index;
        }

        [Route("imports/index")]
        public void PutIndex(int index)
        {
            DeleteIndex();
            ImportManager.Current.SetIndex(null, index);
            Service1.SetNextIndex(index);
        }

        [Route("imports/index")]
        public bool DeleteIndex()
        {
            return ImportManager.Current.DeleteIndex();
        }
    }
}
