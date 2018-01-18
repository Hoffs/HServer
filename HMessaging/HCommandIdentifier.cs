using System;

namespace HServer.HMessaging
{
    public class HCommandIdentifier
    {
        private enum IdentifierType
        {
            Proto = 0,
            String = 1
        }

        private readonly int _protoType;
        private readonly string _stringType;
        private readonly IdentifierType _type;

        public HCommandIdentifier(int protoType)
        {
            _protoType = protoType;
            _type = IdentifierType.Proto;
        }

        public HCommandIdentifier(string stringType)
        {
            _stringType = stringType;
            _type = IdentifierType.String;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int hash = 83; // 41
                switch (_type)
                {
                    case IdentifierType.Proto:
                        return hash * 41 + _protoType.GetHashCode();
                    case IdentifierType.String:
                        return hash * 41 + _stringType.GetHashCode();
                    default:
                        throw new NullTypeException();
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is HCommandIdentifier comparing 
                   && ((_type == IdentifierType.Proto && _protoType.Equals(comparing._protoType)) 
                   || (_type == IdentifierType.String && _stringType.Equals(comparing._stringType)));
        }
    }

    internal class NullTypeException : Exception
    {
    }
}
