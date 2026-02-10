using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementBLL.ViewModels.PlanViewModels
{
    public class UpdatePlanViewModel
    {
        public string Name { get; set; } = null!;
        [Required(ErrorMessage ="Description is Required")]
        [StringLength(200,MinimumLength =5,ErrorMessage ="Description must be between 5 and 200 Chars")]
        public string Description { get; set; } = null!;
        [Required(ErrorMessage ="Duration Days is Required")]
        [Range(1,365,ErrorMessage ="Duration Days must be between 1 and 365")]
        public int DurationDays { get; set; }
        [Required(ErrorMessage ="Price is Required")]
        [Range(0.1,10000,ErrorMessage ="Price must be between 0.1 and 10000")]
        public decimal Price { get; set; }
    }
}
