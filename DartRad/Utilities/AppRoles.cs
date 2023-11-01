namespace DartRad.Utilities
{
    public static class AppRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Editor = "Editor";
        public const string ContentCreator = "ContentCreator";

        public static List<string> ToList()
        {
            return new List<string> { ContentCreator , Editor ,SuperAdmin  };
        }
    }
}
