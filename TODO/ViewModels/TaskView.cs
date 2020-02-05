using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TODO.Attributes;

namespace TODO.ViewModels
{
    //Convertation form for Task
    //Using for sending  info between server and clientside
    public class TaskView
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        
        
        [DataType(DataType.Date)]
        //[PresentFutureDate]
        public string Date { get; set; }
       
        [DataType(DataType.Date)]
        public string CreationDate { get; set; }

        public bool IsComleted { get; set; }
        
        public string Importance { get; set; }
        [Required]
        public int CustomListId { get; set; }
    }
}
