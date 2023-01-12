using System;


namespace Newtonsoft.Json
{
    /// <summary>
    /// Instructs the <see cref="JsonSerializer"/> to use the specified <see cref="JsonCoerceHandler"/>
    /// when serializing the members of class/struct/record.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonCoerceAttribute : Attribute
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the <see cref="JsonCoerceHandler"/>.
        /// </summary>
        /// <value>The <see cref="Type"/> of the <see cref="JsonCoerceHandler"/>.</value>
        public Type CoerceHandlerType { get; }

        /// <summary>
        /// The parameter list to use when constructing the <see cref="JsonCoerceHandler"/>
        /// described by <see cref="CoerceHandlerType"/>.
        /// If <c>null</c>, the default constructor is used.
        /// </summary>
        public object[]? CoerceHandlerParameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoerceAttribute"/> class.
        /// </summary>
        /// <param name="coerceHandlerType">Type of the <see cref="JsonCoerceHandler"/>.</param>
        public JsonCoerceAttribute(Type coerceHandlerType)
        {
            if (coerceHandlerType == null)
            {
                throw new ArgumentNullException(nameof(coerceHandlerType));
            }

            if (!typeof(JsonCoerceHandler).IsAssignableFrom(coerceHandlerType))
            {
                throw new ArgumentException($"No assignable from {nameof(JsonCoerceHandler)}", nameof(coerceHandlerType));
            }

            CoerceHandlerType = coerceHandlerType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCoerceAttribute"/> class.
        /// </summary>
        /// <param name="coerceHandlerType">Type of the <see cref="JsonCoerceHandler"/>.</param>
        /// <param name="coerceHandlerParameters">Parameter list to use when constructing the <see cref="JsonCoerceHandler"/>. Can be <c>null</c>.</param>
        public JsonCoerceAttribute(Type coerceHandlerType, params object[] coerceHandlerParameters)
            : this(coerceHandlerType)
        {
            CoerceHandlerParameters = coerceHandlerParameters;
        }
    }
}
