using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly DataContext _dataContext;
    public UsersController(DataContext dataContext)
    {
      _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
        return await _dataContext.Users.ToListAsync();
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<AppUser>> GetUser(int id){
        return await _dataContext.Users.FindAsync(id);
    }
  }
}