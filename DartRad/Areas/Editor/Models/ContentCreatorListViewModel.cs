using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.Editor.Models
{
    public class ContentCreatorListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CreatedAt { get; set; }
        public string InvitedByAdmin { get; set; }
    }
}
