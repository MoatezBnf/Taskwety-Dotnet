using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskwety_Dotnet.Model;
using Taskwety_Dotnet.Model.Authentication.SignUp;

namespace Taskwety_Dotnet.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/Admin/Users
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");

            var userModels = users.Select(u => new UserModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            }).ToList();

            return Ok(userModels);
        }

        // GET: api/Admin/Users/{id}
        [HttpGet("get-user-by-id/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userModel = new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return Ok(userModel);
        }

        // PUT: api/Admin/Users/{id}
        [HttpPut("edit-user/{id}")]
        public async Task<IActionResult> PutUserModel(string id, UserModel userModel)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = userModel.UserName;
            user.Email = userModel.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = "User Modified Successfully." });
            }

            return BadRequest(result.Errors);
        }

        // POST: api/Admin/Users
        [HttpPost("create-user")]
        public async Task<ActionResult<UserModel>> PostUserModel(RegisterUser registerUser)
        {
            var user = new IdentityUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var userModel = new UserModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userModel);
            }

            return BadRequest(result.Errors);
        }


        // DELETE: api/Admin/Users/{id}
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUserModel(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = "User Deleted Successfully." });
            }

            return BadRequest(result.Errors);
        }
    }
}
