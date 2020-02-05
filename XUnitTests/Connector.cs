using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests
{
    //helping class for connecting test to database
    class Connector
    {
        public static TODO.Models.TodoContext Connect()
        {
            var options = new DbContextOptionsBuilder<TODO.Models.TodoContext>()
            .UseInMemoryDatabase(databaseName: "TodoDb")
            .Options;

            var context = new TODO.Models.TodoContext(options);
            return context;
        }
    }
}
