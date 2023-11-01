namespace DartRad.Areas.ContentCreator.Models
{
    public class AnswerMatchingViewModel
    {
        public int Id { get; set; }
        public string LeftSide { get; set; }
        public string RightSide { get; set; }
    }

    public class AnswerHotspotViewModel
    {
        public int Id { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
