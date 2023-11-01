using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.Editor.Models
{
    public class PendingQuizListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string ImageUrl { get; set; }
    }
}
