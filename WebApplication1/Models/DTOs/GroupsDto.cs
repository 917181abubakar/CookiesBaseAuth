using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApi.Models.DTOs
{
    public class GroupsDto
    {

        public int? Groupid { get; set; }
        public string? GroupName { get; set; }

    }
}
