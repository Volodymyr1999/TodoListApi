using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.Models
{
    public class CustomList
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Task> Tasks { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        
        public CustomList()
        {
            Tasks = new List<Task>();
        }
    }
}
