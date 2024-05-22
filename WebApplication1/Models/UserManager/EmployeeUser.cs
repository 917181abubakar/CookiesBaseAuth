using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using TestApi.Data;

namespace TestApi.Models.UserManager
{
    public class EmployeeUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Employeeid { get; set; }
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public ICollection<UserGroups>? User_Groups { get; set; }

    }

}

