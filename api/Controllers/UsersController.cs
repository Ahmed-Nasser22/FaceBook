using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{

    public class UsersController : BaseApiController
    {
        private readonly DataContext _Context;

     public UsersController(DataContext Context)
     {
            _Context = Context;
        
     }   


     [HttpGet]
    [AllowAnonymous]
     public  async Task<ActionResult<IEnumerable<AppUser>>>  GetUsers ()
     {
          return await    _Context.Users.ToListAsync();
     }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<AppUser>> GetUser (int id)
    {
        return await _Context.Users.FindAsync(id);
    }
    
    }
}