using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestApi.Data;
using TestApi.Models.AuthModels.DTOs;
using TestApi.Models.DTOs;
using TestApi.Models.UserManager;
using Microsoft.AspNetCore.Identity.Data;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;


        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;


        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered and logged in successfully" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return bad request with validation errors
            }

            var user = await _userManager.FindByEmailAsync(model.Email); // Find user by email

            // Validate password (assuming password hashing is implemented)
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Login successful! Create the ClaimsPrincipal object
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID claim
            // Add additional claims as needed (e.g., roles, permissions)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authProperties = new AuthenticationProperties   
                {
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,authProperties);

                return Ok(); // Login successful response (optional)
            }
            else
            {
                // Invalid login attempt
                return Unauthorized("Invalid login credentials.");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpUserDto>>> Getusers()
        {
            var users = await _context.emp_users
                .Include(u => u.User_Groups)
                .ThenInclude(ug => ug.user_Groups)
                .ToListAsync();

            var userdtos = users.Select(
                u => new EmpUserDto
                {
                    Employeeid = u.Employeeid,
                    Email = u.Email,
                    firstname = u.firstname,
                    lastname = u.lastname,
                    User_Groupsdto = u.User_Groups.Select(g => new GroupsDto
                    {
                        Groupid = g.GroupId,
                        GroupName = g.user_Groups?.GroupName

                    }).ToList(),
                }
                ).ToList();
            return Ok(userdtos);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EmpUserDto>>> Getusers(int id)
        {
            var users = await _context.emp_users
                .Include(u => u.User_Groups)
                .ThenInclude(ug => ug.user_Groups)
                .FirstOrDefaultAsync(u => u.Employeeid == id);

            if (users == null)
            {
                return NotFound();
            }

            var userdtos = new EmpUserDto
            {
                Employeeid = users.Employeeid,
                Email = users.Email,
                firstname = users.firstname,
                lastname = users.lastname,
                User_Groupsdto = users.User_Groups.Select(g => new GroupsDto
                {
                    Groupid = g.GroupId,
                    GroupName = g.user_Groups.GroupName

                }).ToList()
            };
            return Ok(userdtos);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.emp_users
                .Include(u => u.User_Groups) // Include related user groups for deletion
                .FirstOrDefaultAsync(u => u.Employeeid == id);

            if (user == null)
            {
                return NotFound();
            }

            // Remove user group associations first (optional for cascading deletes)
            if (user.User_Groups != null)
            {
                _context.RemoveRange(user.User_Groups); // Remove all associated groups
            }

            // Remove the user from the database
            _context.emp_users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // Indicate successful deletion without content
        }

       
           
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] EmpUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }
            var userGroups = new List<UserGroups>();

            if (userDto.User_Groupsdto != null)
            {
                foreach (var groupDto in userDto.User_Groupsdto)
                {
                    var groupId = _context.Groups
                                          .Where(g => g.GroupName.ToLower() == groupDto.GroupName.ToLower())
                                          .Select(g => g.Groupid)
                                          .FirstOrDefault();

                    if (groupId == 0)
                    {
                        return BadRequest($"Group '{groupDto.GroupName}' does not exist.");
                    }

                    userGroups.Add(new UserGroups
                    {
                        GroupId = groupId
                    });
                }
            }

            var user = new EmployeeUsers
            {
                Email = userDto.Email,
                firstname = userDto.firstname,
                lastname = userDto.lastname,
                User_Groups = userGroups
            };

            _context.emp_users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);

        }

    }

}
