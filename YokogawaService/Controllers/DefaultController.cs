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

using System.Web.Http;
using YokogawaService.Models;

namespace YokogawaService.Controllers
{
    public class DefaultController : ApiController
    {
        [Route("")]
        public string Get()
        {
            return "yokogawasvc";
        }

        [Route("summary")]
        public ServiceSummary GetSummary()
        {
            var uow = Request.GetUnitOfWork();
            ServiceSummary result = new ServiceSummary();
            result.TotalFiles = FileUtility.GetFileCount();
            result.TotalImports = uow.GetFileImportCount();
            result.TotalDataRecords = uow.GetMeterDataCount();
            result.CurrentIndex = uow.GetMaxFileIndex().GetValueOrDefault(-1);
            result.LastFile = FileUtility.GetFile(result.CurrentIndex);
            result.LastImport = uow.GetFileImport(result.CurrentIndex);
            return result;
        }

        [Route("index")]
        public ImportIndex GetIndex()
        {
            lock (ImportIndex.Instance)
            {
                return ImportIndex.Instance;
            }
        }

        [Route("index")]
        public void PutIndex([FromBody] ImportIndex index)
        {
            lock (ImportIndex.Instance)
            {
                ImportIndex.Instance.Value = index.Value;
            }
        }

        [HttpGet, Route("index/increment")]
        public ImportIndex IncrementIndex()
        {
            lock (ImportIndex.Instance)
            {
                ImportIndex.Instance.Increment();
                return ImportIndex.Instance;
            }
        }
    }
}
