using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODO;

namespace TODO.Services
{
    //Parent class for different kind of services
    public abstract class Service
    {
        protected Models.TodoContext db;
        public Service(Models.TodoContext db)
        {
            this.db = db;
        }
    }
}
