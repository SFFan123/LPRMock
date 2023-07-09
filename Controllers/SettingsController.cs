using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using LPRMock.Models;
using LPRMock.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LPRMock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        // GET: api/<SettingsController>
        [HttpGet("LPREndpoint")]
        public ActionResult<string?> LprEndpoint()
        {
            try
            {
                var hostedServices = Program.Services.GetServices<IHostedService>();
                var lprService = hostedServices.FirstOrDefault(x => x is LPRService);
                if (lprService != null)
                {
                    return ((LPRService)lprService).BoundEndpoint;
                }
                else
                {
                    return Problem();
                }

            }
            catch (Exception e)
            {
                return Problem();
            }
        }

        [HttpGet(nameof(PrintFilter.allowedPrinternames))]
        public ActionResult<List<string>> GetAllowedPrinternames()
        {
            return PrintFilter.allowedPrinternames;
        }

        [HttpPut(nameof(PrintFilter.allowedPrinternames))]
        public ActionResult AddAllowedPrinterName([Required, MinLength(1)]string printername)
        {
            var regex = new Regex(@"^\w+$");
            if (!regex.IsMatch(printername))
            {
                return BadRequest();
            }

            if (PrintFilter.allowedPrinternames.Contains(printername))
            {
                return Conflict();
            }
            PrintFilter.allowedPrinternames.Add(printername);
            return Ok();
        }

        [HttpDelete (nameof(PrintFilter.allowedPrinternames))]
        [HttpDelete (nameof(PrintFilter.allowedPrinternames)+"/{printername}")]
        public ActionResult RemoveAllowedPrinterName(string? printername)
        {
            if (printername == null)
            {
                PrintFilter.allowedPrinternames.Clear();
                return NoContent();
            }
            else
            {
                var regex = new Regex(@"^\w+$");
                if (!regex.IsMatch(printername))
                {
                    return BadRequest();
                }

                if (!PrintFilter.allowedPrinternames.Contains(printername))
                {
                    return NotFound();
                }
                PrintFilter.allowedPrinternames.Remove(printername);
                return NoContent();
            }
        }


        [HttpGet(nameof(PrintFilter.allowedUsers))]
        public ActionResult<List<string>> GetAllowedUsers()
        {
            return PrintFilter.allowedUsers;
        }

        [HttpPut(nameof(PrintFilter.allowedUsers))]
        public ActionResult AddAllowedUser([Required, MinLength(1)]string username)
        {
            var regex = new Regex(@"^\w+$");
            if (!regex.IsMatch(username))
            {
                return BadRequest();
            }

            if (PrintFilter.allowedUsers.Contains(username))
            {
                return Conflict();
            }
            PrintFilter.allowedUsers.Add(username);
            return Ok();
        }

        [HttpDelete (nameof(PrintFilter.allowedUsers))]
        [HttpDelete (nameof(PrintFilter.allowedUsers)+"/{user}")]
        public ActionResult RemoveAllowedUser(string? user)
        {
            if (user == null)
            {
                PrintFilter.allowedUsers.Clear();
                return NoContent();
            }
            else
            {
                var regex = new Regex(@"^\w+$");
                if (!regex.IsMatch(user))
                {
                    return BadRequest();
                }

                if (!PrintFilter.allowedUsers.Contains(user))
                {
                    return NotFound();
                }
                PrintFilter.allowedUsers.Remove(user);
                return NoContent();
            }
        }


        /// <summary>
        /// Empty allows all.
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(PrintFilter.allowedHosts))]
        public ActionResult<List<string>> GetAllowedHosts()
        {
            return PrintFilter.allowedHosts;
        }

        [HttpPut(nameof(PrintFilter.allowedHosts))]
        public ActionResult AddAllowedHost([Required, MinLength(1)]string hostname)
        {
            var regex = new Regex(@"^\w+$");
            if (!regex.IsMatch(hostname))
            {
                return BadRequest();
            }

            if (PrintFilter.allowedHosts.Contains(hostname))
            {
                return Conflict();
            }
            PrintFilter.allowedHosts.Add(hostname);
            return Ok();
        }

        [HttpDelete (nameof(PrintFilter.allowedHosts))]
        [HttpDelete (nameof(PrintFilter.allowedHosts)+"/{hostname?}")]
        public ActionResult RemoveAllowedHost(string? hostname)
        {
            if (hostname == null)
            {
                PrintFilter.allowedHosts.Clear();
                return NoContent();
            }
            else
            {
                var regex = new Regex(@"^\w+$");
                if (!regex.IsMatch(hostname))
                {
                    return BadRequest();
                }

                if (!PrintFilter.allowedHosts.Contains(hostname))
                {
                    return NotFound();
                }
                PrintFilter.allowedHosts.Remove(hostname);
                return NoContent();
            }
        }
    }
}
