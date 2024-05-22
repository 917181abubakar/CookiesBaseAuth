using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace TestApi.Models.UserManager
{
    public class UserGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [ForeignKey("EmpUser")]
        public int UserId { get; set; }
        public EmployeeUsers? EmpUser { get; set; }

        [ForeignKey("user_Groups")]
        public int GroupId { get; set; }
        public Groups? user_Groups { get; set; }
    }
}
