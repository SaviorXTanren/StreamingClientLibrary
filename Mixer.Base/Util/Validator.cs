using System;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.Util
{
    public static class Validator
    {
        public static void ValidateString(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(name + " must have a valid value");
            }
        }

        public static void ValidateVariable(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentException(name + " is null");
            }
        }

        public static void ValidateList<T>(IEnumerable<T> value, string name)
        {
            Validator.ValidateVariable(value, name);
            if (value.Count() == 0)
            {
                throw new ArgumentException(name + " is empty");
            }
        }
    }
}
