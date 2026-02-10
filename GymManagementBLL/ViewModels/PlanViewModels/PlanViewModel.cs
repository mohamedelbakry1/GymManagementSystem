using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.ViewModels.PlanViewModels
{
    public class PlanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
