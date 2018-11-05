using System;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.Util
{
    /// <summary>
    /// Variable validation helper class
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Validates whether the specified string has a non-null and non-empty value. Throws an ArgumentException if it does not.
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <param name="name">The name of the string</param>
        public static void ValidateString(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(name + " must have a valid value");
            }
        }

        /// <summary>
        /// Validates whether the specified variable is not null. Throws an ArgumentException if it is.
        /// </summary>
        /// <param name="value">The variable to check</param>
        /// <param name="name">The name of the variable</param>
        public static void ValidateVariable(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentException(name + " is null");
            }
        }

        /// <summary>
        /// Validates whether the specified list has is not null and empty. Throws an ArgumentException if it is.
        /// </summary>
        /// <param name="value">The list to check</param>
        /// <param name="name">The name of the list</param>
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
