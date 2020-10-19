using System;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public struct UUID : IComparable, IComparable<UUID>, IEquatable<UUID>
    {
        [ContextMenuItem("New UUID", "NewUUID")]
        public string value;

        private UUID(string value)
        {
            this.value = value;
        }

        [Obsolete]
        public static implicit operator UUID(Guid guid)
        {
            return new UUID(guid.ToString());
        }

        [Obsolete]
        public static implicit operator Guid(UUID uuid)
        {
            if (string.IsNullOrEmpty(uuid.value) || uuid.value == null) { return Guid.Empty; }
            else
            {
                return new Guid(uuid.value);
            }
        }

        [Obsolete]
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            if (!(value is UUID))
            {
                throw new ArgumentException("Must be Guid");
            }

            UUID guid = (UUID)value;
            return guid.value == this.value ? 0 : 1;
        }

        [Obsolete]
        public int CompareTo(UUID other)
        {
            return other.value == value ? 0 : 1;
        }

        public bool Equals(UUID other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value != null ? value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(value) ? new Guid(value).ToString() : string.Empty;
        }


        [Obsolete]
        public static UUID Empty = Guid.Empty;
        /// <summary> A special UUID for player's object </summary>
        [Obsolete]
        public static UUID Player = new UUID("63c5918a-5aad-45ae-8bab-1b681601289b");
        [Obsolete]
        public static UUID NewUUID()
        {
            return Guid.NewGuid();
        }

        public static bool operator ==(UUID u1, UUID u2)
        {
            return u1.Equals(u2);
        }
        [Obsolete]
        public static bool operator !=(UUID u1, UUID u2)
        {
            return !(u1 == u2);
        }

        [Obsolete]
        public static implicit operator string(UUID u)
        {
            return u.value;
        }
    }
}