using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementBLL.ViewModels.TrianerViewModels
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Specialities { get; set; } = null!;
        public string? DateOfBirth { get; set; }
        public string? Address { get; set; }
    }
}
