using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeEntropyGenerator
{
    public struct Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public bool IsIdentifier
        {
            get;
            private set;
        }

        public bool IsQueryable
        {
            get;
            private set;
        }

        public Attribute(
            string name,
            bool isIdentifier = false,
            bool isQueryable = false) 
            : this()
        {
            Name = name;
            IsIdentifier = isIdentifier;
            IsQueryable = isQueryable;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^
                (IsIdentifier ? 2309 << 5 : 0) ^
                (IsQueryable ? 835285 : 0);
        }

        public static bool operator ==(Attribute a1, Attribute a2)
        {
            return
                a1.Name == a2.Name &&
                a1.IsIdentifier == a2.IsIdentifier &&
                a1.IsQueryable == a2.IsQueryable;
        }

        public static bool operator !=(Attribute a1, Attribute a2)
        {
            return !(a1 == a2);
        }

        public override bool Equals(object obj)
        {
            if(obj is Attribute)
            {
                return this == (Attribute)obj;
            }
            else
            {
                return false;
            }
        }
    }
}
