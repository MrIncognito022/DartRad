using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.ContentCreator.Models
{
    public class QuizCreateViewModel
    {
        public QuizCreateViewModel()
        {
            Questions = new List<QuestionListViewModel>();

        }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Clinical Scenario")]
        public string ClinicalScenario { get; set; }

        public List<QuestionListViewModel> Questions { get; set; }

        [Display(Name = "Quiz Image")]
        public IFormFile QuizImage { get; set; }
        [Required]
        public string Category { get; set; }

        public List<SelectListItem> QuizCategories
        {
            get
            {
                return SD.QuizCategories.Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x
                }).ToList();
            }
        }
    }

}
