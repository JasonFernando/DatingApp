using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccountController : ApiBaseController
  {
    private readonly DataContext _dataContext;
    private readonly ITokenService _TokenService;
    public AccountController(DataContext dataContext, ITokenService TokenService)
    {
      _TokenService = TokenService;
      _dataContext = dataContext;
    }
    [HttpPost("Register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto userRegister)
    {
      using var hmac = new HMACSHA512();

      if (await UserExist(userRegister.UserName.ToLower())) return BadRequest("User is taken.");

      var user = new AppUser()
      {
        UserName = userRegister.UserName.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRegister.Password)),
        PasswordSalt = hmac.Key
      };

      _dataContext.Users.Add(user);
      await _dataContext.SaveChangesAsync();
      
      return new UserDto{
        UserName = user.UserName,
        Token = _TokenService.CreateToken(user)  
      };
    }

    [HttpPost("Login")]
    public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
    {
      var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

      if (user == null) return Unauthorized("Invalid username.");

      using var hmac = new HMACSHA512(user.PasswordSalt);
      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      for (var i = 0; i < computedHash.Length; i++)
      {
        if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
      }
      return user;
    }

    private async Task<bool> UserExist(string userName)
    {
      return await _dataContext.Users.AnyAsync(x => x.UserName == userName.ToLower());
    }
  }
}