using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.Models
{
    /// <summary>
    /// Importances 'hight','normal,'low'
    /// </summary>
    //Importances low|normal|hight
    public class Importance:IComparable
    {
        /// <summary>
        /// id 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Importance name for example: 'hight','normal,'low'
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// number representation of Importance is used for sorting
        /// </summary>
        public int DigitValue { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public List<Task> Tasks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Importance()
        {
            Tasks = new List<Task>();
        }
       /// <summary>
       /// Compare two Importances
       /// </summary>
       /// <param name="obj"></param>
       /// <returns></returns>
        public int CompareTo(object obj)
        {
            Importance imp = obj as Importance;
            return DigitValue.CompareTo(imp.DigitValue);
        }
    }
}
