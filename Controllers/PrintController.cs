using LPRMock.Models;
using LPRMock.Services;
using Microsoft.AspNetCore.Mvc;

namespace LPRMock.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PrintController : ControllerBase
    {

        [HttpGet]
        public ActionResult<List<PrintJob>> List()
        {
            return Ok(Program.Jobs);
        }

        [HttpGet]
        public ActionResult<List<PrintJob>> Get(int id)
        {
            PrintJob? job = Program.Jobs.FirstOrDefault(x => x.JobNumber == id);

            if (job == null)
            {
                return NoContent();
            }
            return Ok(job);
        }


        [HttpDelete]
        public ActionResult PurgeQueue()
        {
            Program.Jobs.Clear();
            return NoContent();
        }
    }
}
