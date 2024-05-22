using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApi.Models.UserManager
{
    public class Groups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Groupid { get; set; }
        public string? GroupName { get; set; }

        public ICollection<UserGroups>? User_Groups { get; set; }
    }
}
