using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_Management.Model
{
    public class Credential
    {
        public Credential() { }
        public Credential(string description, string username, string domain, string password)
        {
            Description = description;
            Username = username;
            Domain = domain;
            Password = password;
            /*
            foreach (char c in password)
                Password.AppendChar(c);
            */
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
