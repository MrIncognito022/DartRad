using System.ComponentModel.DataAnnotations;
using DartRad.Areas.Editor.Models;

namespace DartRad.Areas.ContentCreator.Models
{
    public class QuizDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Clinical Scenario")]
        public string ClinicalScenario { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Status")]
        public QuizStatus Status { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public List<QuizNoteViewModel> Notes { get; set; }
    }

}
