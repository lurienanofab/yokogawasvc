using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using YokogawaService.Models;

namespace YokogawaService.Controllers
{
    public class ReportController : ApiController
    {
        [Route("report")]
        public IEnumerable<Report> Get()
        {
            return Request.GetUnitOfWork().GetReports();
        }

        [Route("report/active")]
        public IEnumerable<Report> GetActive()
        {
            return Request.GetUnitOfWork().GetReports().Where(x => x.Active);
        }

        [HttpPost, Route("report/run/{type}")]
        public ReportModel RunReport([FromBody] DataQueryCriteria criteria, [FromUri] string type)
        {
            return Request.GetUnitOfWork().RunReport(criteria, type);
        }
    }
}
