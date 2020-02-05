using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.Models
{
    public class User 
    { 
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
        
        public List<Task> Tasks { get; set; }
        public List<CustomList> customLists { get; set; }
        public User()
        {
            customLists = new List<CustomList>();
        }
    }
}
