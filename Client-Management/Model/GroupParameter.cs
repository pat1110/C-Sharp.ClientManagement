using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_Management.Model
{

    public class GroupParameter
    {
        public GroupParameter() { }
        public GroupParameter(string connection, string condition, string property, string search)
        {
            Connection = connection;
            Condition = condition;
            Property = property;
            Search = search;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Connection { get; set; }
        public string Property { get; set; }
        public string Condition { get; set; }
        public string Search { get; set; }
    }
}
