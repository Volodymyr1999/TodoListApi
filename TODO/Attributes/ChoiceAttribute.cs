using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TODO.Attributes
{
    public class ChoiceAttribute:ValidationAttribute
    {
        object[] choices;
        public ChoiceAttribute(object[] arr)
        {
            choices = arr;
        }

        public override bool IsValid(object value)
        {
            return choices.Contains(value);
        }
    }
}
