using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementBLL.ViewModels.UserViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name ="First Name")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage ="Password is Required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage ="Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = null!;
    }
}
