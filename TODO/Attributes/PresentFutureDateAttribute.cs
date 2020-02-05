using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TODO.Attributes
{
    public class PresentFutureDateAttribute:ValidationAttribute
    {
        public  bool IsValid(DateTime value)
        {
            return value.Date >= DateTime.Today;
        }
    }
}
