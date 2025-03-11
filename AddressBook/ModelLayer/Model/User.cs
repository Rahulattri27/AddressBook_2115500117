using System;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Model
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string PasswordHash { get; set; }
	}
}

