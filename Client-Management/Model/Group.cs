using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_Management.Model
{
    public class Group
    {
        public Group()
        {
            Parameter = new List<GroupParameter>();
        }
        public Group(string name, string description, List<GroupParameter> parameter)
        {
            Name = name;
            Description = description;
            if (parameter != null)
                Parameter = parameter;
            else
                Parameter = new List<GroupParameter>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GroupParameter> Parameter { get; set; }
    }
}
