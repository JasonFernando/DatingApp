using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class UsersController : ApiBaseController
  {
    private readonly DataContext _dataContext;
    public UsersController(DataContext dataContext)
    {
      _dataContext = dataContext;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
        return await _dataContext.Users.ToListAsync();
    }

    [Authorize]
    [HttpGet("{Id}")]
    public async Task<ActionResult<AppUser>> GetUser(int id){
        return await _dataContext.Users.FindAsync(id);
    }
  }
}