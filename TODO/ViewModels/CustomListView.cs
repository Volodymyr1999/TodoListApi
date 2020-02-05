using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.ViewModels
{
    //Convertation form for CustomList
    //Using for sending  info between server and clientside
    public class CustomListView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TaskView> taskViews { get; set; }
    }
}
