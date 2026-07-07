using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Domain.Entities
{
    public class Departments
    {
        public Guid Id { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
          public ICollection<Employees> Employees { get; set; }
        = new List<Employees>();
    }
}
