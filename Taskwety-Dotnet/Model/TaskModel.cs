using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Taskwety_Dotnet.Model
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public string? TaskDescription { get; set;}
        [ForeignKey("UserId")]
        public string? UserId { get; set; }
    }
}
