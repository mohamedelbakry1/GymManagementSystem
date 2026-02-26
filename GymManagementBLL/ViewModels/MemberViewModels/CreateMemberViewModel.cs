using GymManagementDAL.Entities.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementBLL.ViewModels.MemberViewModels
{
    public class CreateMemberViewModel
    {
        [Required(ErrorMessage ="Profile Photo is Required")]
        [Display(Name="Profile Photo")]
        public IFormFile PhotoFile { get; set; } = null!;
        [Required(ErrorMessage = "Name is Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 Chars")]
        [RegularExpression(@"[a-zA-Z\s]+$", ErrorMessage = "Name can contain only letters and spaces")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100,MinimumLength =5,ErrorMessage ="Email must be between 5 and 100 Chars")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Phone is Required")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Invalid Phone Format")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone Number must be valid Egyptian PhoneNumber")]
        public string Phone { get; set; } = null!;
        [Required(ErrorMessage ="Date of Birth is Required")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        [Required(ErrorMessage ="Gender is Required")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage ="Building Number is Required")]
        [Range(1,9000,ErrorMessage ="Building Number must be between 1 and 9000")]
        public int BuildingNumber { get; set; }
        [Required(ErrorMessage ="Street is Required")]
        [StringLength(30,MinimumLength =2,ErrorMessage ="Street must be between 2 and 30 Chars")]
        public string Street { get; set; } = null!;
        [Required(ErrorMessage = "City is Required")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "City must be between 2 and 30 Chars")]
        [RegularExpression(@"[a-zA-Z\s]+$", ErrorMessage = "City can contain only letters and spaces")]
        public string City { get; set; } = null!;
        [Required(ErrorMessage ="Health Record is Required")]
        public HealthRecordViewModel HealthRecordViewModel { get; set; } = null!;

    }
}
