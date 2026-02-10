using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementBLL.ViewModels.MemberViewModels
{
    public class HealthRecordViewModel
    {
        [Required(ErrorMessage ="Height is Required")]
        [Range(0.1,300,ErrorMessage ="Height must be greater than 0.1 and less than 300")]
        public decimal Height { get; set; }
        [Required(ErrorMessage = "Weight is Required")]
        [Range(0.1, 300, ErrorMessage = "Weight must be greater than 0.1 and less than 300")]
        public decimal Weight { get; set; }
        [Required(ErrorMessage ="BloodType is Required")]
        [StringLength(3,ErrorMessage ="Blood Type must be 3 Char or less")]
        public string BloodType { get; set; } = null!;
        public string? Note { get; set; }
    }
}
