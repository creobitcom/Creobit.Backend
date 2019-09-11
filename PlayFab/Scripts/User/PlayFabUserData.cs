#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Creobit.Backend.User
{
    public sealed class PlayFabUserData : IPlayFabUserData
    {
        #region IUserData

        void IUserData.Read<T>(Action<T> onComplete, Action onFailure)
        {
            try
            {
                if (IsReadOnly(typeof(T)))
                {
                    PlayFabClientAPI.GetUserReadOnlyData(
                        new GetUserDataRequest()
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
                            PlayFabErrorHandler.Process(error);

                            onFailure();
                        });
                }
                else
                {
                    PlayFabClientAPI.GetUserData(
                        new GetUserDataRequest()
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
                            PlayFabErrorHandler.Process(error);

                            onFailure();
                        });
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        void IUserData.Write(object data, Action onComplete, Action onFailure)
        {
            var type = data.GetType();
            var serializedData = SerializeData(data, type);
            var keysToRemove = new List<string>();

            foreach (var item in serializedData)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    keysToRemove.Add(item.Key);
                }
            }

            foreach (var item in keysToRemove)
            {
                serializedData.Remove(item);
            }

            try
            {
                PlayFabClientAPI.UpdateUserData(
                    new UpdateUserDataRequest()
                    {
                        Data = serializedData,
                        KeysToRemove = keysToRemove,
                        Permission = IsPublic(type) ? UserDataPermission.Public : UserDataPermission.Private
                    },
                    result =>
                    {
                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        #endregion
        #region PlayFabUserData

        private readonly Dictionary<Type, List<Accessor>> Accessors = new Dictionary<Type, List<Accessor>>();
        private readonly HashSet<MethodInfo> MethodInfos = new HashSet<MethodInfo>();

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

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

        private T DeserializeData<T>(IDictionary<string, UserDataRecord> inputData)
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
                            accessor.SetValue(outputData, bool.Parse(value.Value));
                            break;
                        case TypeCode.Int32:
                            accessor.SetValue(outputData, int.Parse(value.Value, NumberStyles.Number, CultureInfo.InvariantCulture));
                            break;
                        case TypeCode.Single:
                            accessor.SetValue(outputData, float.Parse(value.Value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture));
                            break;
                        case TypeCode.DateTime:
                            accessor.SetValue(outputData, DateTime.Parse(value.Value, CultureInfo.InvariantCulture, DateTimeStyles.None).ToLocalTime());
                            break;
                        case TypeCode.String:
                            accessor.SetValue(outputData, value.Value);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return outputData;
        }

        private bool IsPublic(Type type)
        {
            var sourceAttribute = type.GetCustomAttribute<SourceAttribute>();

            return sourceAttribute == null ? false : sourceAttribute.Public;
        }

        private bool IsReadOnly(Type type)
        {
            var sourceAttribute = type.GetCustomAttribute<SourceAttribute>();

            return sourceAttribute == null ? false : sourceAttribute.ReadOnly;
        }

        private Dictionary<string, string> SerializeData(object inputData, Type type)
        {
            var outputData = new Dictionary<string, string>();

            foreach (var accessor in GetAccessors(type))
            {
                var value = accessor.GetValue(inputData);

                outputData.Add(accessor.Key, Convert(value, accessor.TypeCode));
            }

            return outputData;

            string Convert(object value, TypeCode typeCode)
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return ((bool)value).ToString();
                    case TypeCode.Int32:
                        return ((int)value).ToString("G", CultureInfo.InvariantCulture);
                    case TypeCode.Single:
                        return ((float)value).ToString("R", CultureInfo.InvariantCulture);
                    case TypeCode.DateTime:
                        return ((DateTime)value).ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
                    case TypeCode.String:
                        return (string)value;
                    default:
                        throw new NotSupportedException();
                }
            }
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
