using System;
using ChatProtos.Networking;

namespace CoreServer.HMessaging.HCommands
{
    public class HCommandIdentifier
    {
        private readonly RequestType _protoType;
        private readonly string _stringType;
        private readonly ITypeable _objectType;

        public HCommandIdentifier(RequestType protoType)
        {
            _protoType = protoType;
        }

        public HCommandIdentifier(string stringType)
        {
            _stringType = stringType;
        }

        public HCommandIdentifier(ITypeable customType)
        {
            _objectType = customType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 83; // 41
                if (_protoType != null)
                {
                    return hash * 41 + _protoType.GetHashCode();
                } else if (_stringType != null)
                {
                    return hash * 41 + _stringType.GetHashCode();
                } else if (_objectType != null)
                {
                    return hash * 41 + _objectType.GetTypeString().GetHashCode();
                }
                else
                {
                    throw new NullTypeException();
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is HCommandIdentifier comparing 
                   && ((_protoType != null && _protoType.Equals(comparing._protoType)) 
                   || (_objectType != null && _objectType?.GetTypeString().Equals(comparing._objectType?.GetTypeString()) == true)
                   || (_stringType != null && _stringType.Equals(comparing._stringType)));
        }
    }

    internal class NullTypeException : Exception
    {
    }
}
