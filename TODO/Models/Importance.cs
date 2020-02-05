using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.Models
{
    //Importances low|normal|hight
    public class Importance:IComparable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DigitValue { get; set; } //number representation of Importance is used for sorting
        public List<Task> Tasks { get; set; }
        public Importance()
        {
            Tasks = new List<Task>();
        }
       
        public int CompareTo(object obj)
        {
            Importance imp = obj as Importance;
            return DigitValue.CompareTo(imp.DigitValue);
        }
    }
}
