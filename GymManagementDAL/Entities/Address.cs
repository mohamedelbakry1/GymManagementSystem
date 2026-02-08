using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Entities
{
    public class Address
    {
        public int BuildingNumber { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}
