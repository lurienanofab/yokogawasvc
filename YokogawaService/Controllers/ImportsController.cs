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

using System.Collections.Generic;
using System.Web.Http;
using YokogawaService.Models;

namespace YokogawaService.Controllers
{
    public class ImportsController : ApiController
    {
        [Route("imports")]
        public IEnumerable<FileImport> GetImports(int skip = 0, int limit = 100)
        {
            return Request.GetUnitOfWork().AllImportFiles(skip, limit);
        }

        [Route("imports")]
        public int DeleteFileImports()
        {
            return Request.GetUnitOfWork().DeleteFileImports();
        }

        [Route("imports/{index}")]
        public FileImport GetFileImport(int index)
        {
            return Request.GetUnitOfWork().GetFileImport(index);
        }

        [Route("imports/{index}")]
        public bool DeleteFileImport(int index)
        {
            return Request.GetUnitOfWork().DeleteFileImport(index);
        }

        [Route("imports/{index}/data")]
        public IEnumerable<MeterData> GetMeterData(int index)
        {
            return Request.GetUnitOfWork().QueryMeterData(index);
        }

        [Route("imports/data")]
        public IEnumerable<MeterData> GetData(int skip = 0, int limit = 100)
        {
            return Request.GetUnitOfWork().AllMeterData(skip, limit);
        }

        [HttpPost, Route("imports/data")]
        public IEnumerable<MeterData> QueryFileImportData([FromBody] DataQueryCriteria criteria)
        {
            return Request.GetUnitOfWork().QueryMeterData(criteria);
        }

        [Route("imports/last")]
        public FileImport GetLastImport()
        {
            var index = Request.GetUnitOfWork().GetMaxFileIndex();

            if (index.HasValue)
                return Request.GetUnitOfWork().GetFileImport(index.Value);
            else
               return null;
        }

        [Route("imports/last/data")]
        public IEnumerable<MeterData> GetLastMeterData()
        {
            var index = Request.GetUnitOfWork().GetMaxFileIndex();

            if (index.HasValue)
                return Request.GetUnitOfWork().QueryMeterData(index.Value);
            else
                return null;
        }

        [Route("imports/index")]
        public int? GetIndex()
        {
            return Request.GetUnitOfWork().GetMaxFileIndex();
        }
    }
}
