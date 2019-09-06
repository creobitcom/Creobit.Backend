#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Creobit.Backend.Application
{
    public sealed class PlayFabApplicationData : IPlayFabApplicationData
    {
        #region IApplicationData

        void IApplicationData.Read<T>(Action<T> onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetTitleData(
                    new GetTitleDataRequest()
                    {
                        Keys = GetDataKeys<T>()
                    },
                    result =>
                    {
                        var data = DeserializeData<T>(result.Data);

                        onComplete(data);
                    },
                    error =>
                    {
                        PlayFabErrorHandler?.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler?.Process(exception);

                onFailure();
            }
        }

        #endregion
        #region PlayFabApplicationData

        private readonly Dictionary<Type, List<Accessor>> Accessors = new Dictionary<Type, List<Accessor>>();
        private readonly HashSet<MethodInfo> MethodInfos = new HashSet<MethodInfo>();

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get;
            set;
        } = Backend.PlayFabErrorHandler.Default;

        private List<Accessor> CreateAccessors(Type type)
        {
            var accessors = new List<Accessor>();

            CacheProperties();

            return accessors;

            void CacheProperties()
            {
                for (var currentType = type; currentType != null; currentType = currentType.BaseType)
                {
                    var typeInfo = IntrospectionExtensions.GetTypeInfo(currentType);

                    foreach (var declaredProperty in typeInfo.DeclaredProperties)
                    {
                        if (!declaredProperty.CanRead || !declaredProperty.CanWrite)
                        {
                            continue;
                        }

                        var getMethod = declaredProperty.GetGetMethod(true);
                        var baseDefinition = getMethod.GetBaseDefinition();

                        if (MethodInfos.Contains(baseDefinition))
                        {
                            continue;
                        }

                        MethodInfos.Add(baseDefinition);

                        var keyAttribute = declaredProperty.GetCustomAttribute<KeyAttribute>();

                        if (keyAttribute == null)
                        {
                            continue;
                        }

                        var accessor = new Accessor(declaredProperty, keyAttribute);

                        accessors.Add(accessor);
                    }
                }

                MethodInfos.Clear();
            }
        }

        private T DeserializeData<T>(IDictionary<string, string> inputData)
            where T : class, new()
        {
            var outputData = new T();

            foreach (var accessor in GetAccessors(typeof(T)))
            {
                if (inputData.TryGetValue(accessor.Key, out var value))
                {
                    switch (accessor.TypeCode)
                    {
                        case TypeCode.Boolean:
                            accessor.SetValue(outputData, bool.Parse(value));
                            break;
                        case TypeCode.Int32:
                            accessor.SetValue(outputData, int.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture));
                            break;
                        case TypeCode.Single:
                            accessor.SetValue(outputData, float.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture));
                            break;
                        case TypeCode.DateTime:
                            accessor.SetValue(outputData, DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None).ToLocalTime());
                            break;
                        case TypeCode.String:
                            accessor.SetValue(outputData, value);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return outputData;
        }

        private IEnumerable<Accessor> GetAccessors(Type type)
        {
            if (!Accessors.TryGetValue(type, out var accessors))
            {
                Accessors[type] = accessors = CreateAccessors(type);
            }

            return accessors;
        }

        private List<string> GetDataKeys<T>()
            where T : class
        {
            var keys = new List<string>();

            foreach (var accessor in GetAccessors(typeof(T)))
            {
                var key = accessor.Key.ToString();

                keys.Add(key);
            }

            return keys;
        }

        #endregion
    }
}
#endif
