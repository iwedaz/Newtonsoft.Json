using System;



namespace Newtonsoft.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonCoerceAttribute : Attribute
    {
        public Type CoerceHandlerType { get; }
        public object[]? CoerceHandlerParameters { get; }


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

        public JsonCoerceAttribute(Type coerceHandlerType, params object[] converterParameters)
            : this(coerceHandlerType)
        {
            CoerceHandlerParameters = converterParameters;
        }
    }
}
