using System;
using System.Collections.Generic;
using System.Text;

namespace inercya.EntityLite.Collections
{
    internal struct TypeStringKey : IEquatable<TypeStringKey>
    {
        public Type Type { get; private set; }
        public string String { get; private set; }

        public TypeStringKey(Type entityType, string str)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            if (str == null) throw new ArgumentNullException(nameof(str));
            this.Type = entityType;
            this.String = str;
        }

        public bool Equals(TypeStringKey other)
        {
            return Type == other.Type && String == other.String;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeStringKey other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Type.GetHashCode(), this.String.GetHashCode());
        }

        public static bool operator ==(TypeStringKey left, TypeStringKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeStringKey left, TypeStringKey right)
        {
            return !(left == right);
        }
    }
}
