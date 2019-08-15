using System;
using System.Reflection;

namespace Creobit.Backend
{
    public sealed class Accessor
    {
        #region Object

        public override string ToString()
        {
            var declaringType = PropertyInfo.DeclaringType;

            return $"{declaringType.Name}.{PropertyInfo.Name}";
        }

        #endregion
        #region Accessor

        private readonly KeyAttribute KeyAttribute;
        private readonly PropertyInfo PropertyInfo;

        public Accessor(PropertyInfo propertyInfo, KeyAttribute keyAttribute)
        {
            KeyAttribute = keyAttribute;
            PropertyInfo = propertyInfo;
        }

        public string Key => KeyAttribute.Key.ToString();

        public TypeCode TypeCode => Type.GetTypeCode(PropertyInfo.PropertyType);

        public object GetValue(object instance) => PropertyInfo.GetValue(instance);

        public void SetValue(object instance, object value) => PropertyInfo.SetValue(instance, value);

        #endregion
    }
}