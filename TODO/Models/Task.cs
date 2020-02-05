using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using System.Collections;
namespace TODO.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public string DueDate { get; set; }

        [DataType(DataType.Date)]
        public string CreationDate { get; set; }
       
        public bool IsCompleted { get; set; } //if Task is completed it can be soft deleted
        
        
        public int ImportanceId { get; set; }
        public Importance Importance { get; set; }

        public int CustomListId { get; set; }
        public CustomList CustomList { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        
        
    }
}
