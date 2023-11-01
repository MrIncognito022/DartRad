using DartRad.Areas.ContentCreator.Models;

namespace DartRad.Areas.Editor.Models
{
    public class PendingQuizDetailsViewModel : PendingQuizListViewModel
    {
        public PendingQuizDetailsViewModel()
        {
            QuizNotes = new List<QuizNoteViewModel>();
        }
        public string ClinicalScenario { get; set; }
        
        public List<QuizNoteViewModel> QuizNotes { get; set; }
        public List<QuestionListViewModel> Questions { get; set; } 
    }
}
