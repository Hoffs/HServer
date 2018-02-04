namespace HServer.HMessaging
{
    using JetBrains.Annotations;

    /// <summary>
    /// The command identifier.
    /// </summary>
    public class HCommandIdentifier
    {
        /// <summary>
        /// The proto identifier.
        /// </summary>
        private readonly int _proto;

        /// <summary>
        /// The string identifier.
        /// </summary>
        [CanBeNull]
        private readonly string _string;

        /// <summary>
        /// The type of identifier.
        /// </summary>
        [NotNull]
        private readonly IdentifierType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="HCommandIdentifier"/> class.
        /// </summary>
        /// <param name="proto">
        /// The proto identifier.
        /// </param>
        public HCommandIdentifier(int proto)
        {
            _proto = proto;
            _type = IdentifierType.Proto;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HCommandIdentifier"/> class.
        /// </summary>
        /// <param name="stringIdentifier">
        /// The string identifier.
        /// </param>
        public HCommandIdentifier([NotNull] string stringIdentifier)
        {
            _string = stringIdentifier;
            _type = IdentifierType.String;
        }

        /// <summary>
        /// The identifier type.
        /// </summary>
        private enum IdentifierType
        {
            /// <summary>
            /// The proto identifier.
            /// </summary>
            Proto = 0,

            /// <summary>
            /// The string identifier.
            /// </summary>
            String = 1
        }

        /// <summary>
        /// Gets hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="NullTypeException">
        /// If identifier type is not specified.
        /// </exception>
        public override int GetHashCode()
        {
            unchecked
            {
                const int Hash = 83; // 41
                switch (_type)
                {
                    case IdentifierType.Proto:
                        return (Hash * 41) + _proto.GetHashCode();
                    case IdentifierType.String:
                        return (Hash * 41) + _string.GetHashCode();
                    default:
                        throw new NullTypeException();
                }
            }
        }

        /// <summary>
        /// Checks if objects are equals.
        /// </summary>
        /// <param name="obj">
        /// The comparing object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is HCommandIdentifier comparing 
                   && ((_type == IdentifierType.Proto && _proto.Equals(comparing._proto)) 
                   || (_type == IdentifierType.String && _string.Equals(comparing._string)));
        }
    }
}
