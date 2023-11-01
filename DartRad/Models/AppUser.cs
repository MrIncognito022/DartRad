namespace DartRad.Models
{
    /// <summary>
    /// A model to identify the logged in user
    /// </summary>
    public class AppUser
    {

        public AppUser()
        {

        }

        public AppUser(int id,string name, string email, string role)
        {
            Id = id;
            Name = name;
            Email = email;
            Role = role;
        }
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
