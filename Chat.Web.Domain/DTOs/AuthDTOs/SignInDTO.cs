using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Chat.Web.Domain.DTOs.AuthDTOs
{
    public class SignInDTO
    {
        [Required]
        [StringLength(25, ErrorMessage = "Invalid Username (Maximum 25 chars)")]
        public string Username { get; set; }

        [Required]
        [StringLength(25)]
        public string Password { get; set; }
    }
}
