using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.Models
{
    /// <summary>
    /// CustomList
    /// </summary>
    public class CustomList
    {
        /// <summary>
        /// The id of the CustomList created by user
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// name of created list
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tasks which this customlist include
        /// </summary>
        public List<Task> Tasks { get; set; }
        /// <summary>
        /// User's Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// contructor
        /// </summary>
        public CustomList()
        {
            Tasks = new List<Task>();
        }
    }
}
