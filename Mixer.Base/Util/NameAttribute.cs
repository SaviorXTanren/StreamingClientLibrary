using System;

namespace Mixer.Base.Util
{
    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute
    {
        public static readonly NameAttribute Default;

        public string Name { get; private set; }

        public NameAttribute() : this(string.Empty) { }

        public NameAttribute(string name) { this.Name = name; }

        public override bool Equals(object obj)
        {
            if (obj is NameAttribute)
            {
                NameAttribute other = (NameAttribute)obj;
                return this.Name.Equals(other.Name);
            }
            return false;
        }

        public override int GetHashCode() { return this.Name.GetHashCode(); }

        public override bool IsDefaultAttribute() { return this.Equals(NameAttribute.Default); }
    }
}
