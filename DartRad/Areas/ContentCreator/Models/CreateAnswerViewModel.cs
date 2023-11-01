using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.ContentCreator.Models
{
    /// <summary>
    /// TODO: might change this schema depending on upcoming types
    /// </summary>
    public class CreateAnswerViewModel
    {
        
        public CreateGenericAnswerViewModel Answer { get; set; }
        public CreateMatchingAnswerViewModel MatchingAnswer { get; set; }
        public CreateHotspotAnswerViewModel HotspotAnswer { get; set; }

    }

    public class UpdateAnswerViewModel : CreateAnswerViewModel
    {
        [Required]
        public int Id { get; set; }
    }

    public class CreateGenericAnswerViewModel
    {
        // for generic

        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }


    public class CreateMatchingAnswerViewModel
    {
        // for matching
        public string LeftSide { get; set; }
        public string RightSide { get; set; }
    }

    public class CreateHotspotAnswerViewModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class UpdateSequenceViewModel
    {
        public List<int> AnswerIds { get; set; }
    }
}
