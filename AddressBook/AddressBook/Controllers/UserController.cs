using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Interface;
using ModelLayer.Model;
using ModelLayer.DTO;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AddressBook.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class UserController:ControllerBase
	{
		private readonly IUserBL _userBL;
		public UserController(IUserBL userRL)
		{
			_userBL = userRL;
		}


		[HttpPost("register")]
		public IActionResult Register(RegisterDTO registerDTO)
		{
			var user = _userBL.RegisterUser(registerDTO);
			if (user == null)
			{
				return BadRequest("User Already Exist");
			}
			return Ok("User Registered Successfully");
		}
		[HttpPost("login")]
		public IActionResult Login(LoginDTO login)
		{
			var user = _userBL.LoginUser(login);
			if (user == null)
			{
				return Unauthorized("Invalid credentials");
			}
			return Ok(user);
		}
		[Authorize(Roles="Admin")]
		[HttpGet("all-user")]
		public IActionResult GetAllUSer()
		{
			return Ok("Only admin can access this.");
		}
		[HttpPost("forget-password")]
		public IActionResult ForgetPassword([FromBody] string email)
		{
			bool success = _userBL.ForgetPassword(email);
			if (success)
			{
				return Ok("Reset Link Sent to your Email");
			}
			return NotFound("Email Not Found");
		}
		[HttpPost("reset-password")]
		public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPassword)
		{
			bool success = _userBL.ResetPassword(resetPassword.ResetToken, resetPassword.NewPassword);
			if (success)
			{
				return Ok("Password reset Successfully");
			}
			return NotFound("Invalid or Expired token");
		}

        

    }
}

