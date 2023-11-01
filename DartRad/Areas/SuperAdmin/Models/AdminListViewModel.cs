using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.SuperAdmin.Models
{
    public class AdminListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBySuperAdmin { get; set; }
    }
}
