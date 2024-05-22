using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TestApi.Models.UserManager;

namespace TestApi.Models.DTOs
{
    public class EmpUserDto
    {
        public int Employeeid { get; set; }
        public string? Email { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public List<GroupsDto> User_Groupsdto { get; set; }
    }
}
