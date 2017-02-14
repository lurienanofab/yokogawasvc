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

using System.IO;
using System.Net;
using System.Net.Http;
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
            ServiceSummary result = new ServiceSummary();
            result.TotalFiles = FileUtility.GetFileCount();
            result.TotalImports = ImportManager.Current.GetImportCount();
            result.TotalDataRecords = ImportManager.Current.GetImportDataCount();

            var importIndex = ImportManager.Current.GetIndex();

            result.CurrentIndex = -1;

            if (importIndex != null)
                result.CurrentIndex = importIndex.Index;

            result.LastFile = FileUtility.GetFile(result.CurrentIndex);
            result.LastImport = ModelFactory.CreateImportFileModel(ImportManager.Current.GetImportFile(result.CurrentIndex));
            
            return result;
        }
    }
}
