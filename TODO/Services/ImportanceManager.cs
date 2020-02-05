using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODO.Models;
namespace TODO.Services
{
    //For appending Importances to Database
    public class ImportanceManager
    {
        //create Importances: hight,normal,low
        public static void CreateDefaultImportances(TodoContext context)
        {
            if (context.Importances.Count() == 0)
            {
                List<Importance> importances = new List<Importance>
                {
                    new Importance{ Name="low",DigitValue=0},
                    new Importance{Name="normal",DigitValue=1},
                    new Importance{Name="hight",DigitValue=2}
                };
                context.AddRange(importances);
                context.SaveChanges();
            }
        }

        
    }
}
