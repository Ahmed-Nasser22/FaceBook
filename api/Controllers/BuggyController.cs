using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext  context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return " Secret Text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var Anything = _context.Users.Find(-1);
            if(Anything == null)
            {
                return NotFound();
            }
            return Ok(Anything);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            return _context.Users.Find(-1).ToString();
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is a bad reuquest");
        }

    }
}
