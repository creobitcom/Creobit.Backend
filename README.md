# Creobit.Backend.AndroidPlayFab
Модуль для работы с **PlayFab** посредством **Android**.

## Установка
* Выполнить шаги для установки [**Creobit.Backend.PlayFab**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendplayfab);
* Добавить директиву препроцессора **CREOBIT_BACKEND_ANDROID**.

## Инициализация
```csharp
var playFabAuth = new PlayFabAuth(titleId: "");
var playFabLink = new PlayFabLink();

var androidPlayFabAuth = new AndroidPlayFabAuth(playFabAuth);
var androidPlayFabLink = new AndroidPlayFabLink(playFabLink, androidPlayFabAuth);
```

# Creobit.Backend.AppStorePlayFab
Модуль для работы с **PlayFab** посредством **AppStore**.

## Установка
* Выполнить шаги для установки [**Creobit.Backend.PlayFab**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendplayfab);
* Выполнить шаги для установки [**UnityIAP**](https://docs.unity3d.com/Manual/UnityIAPSettingUp.html);
* Добавить директиву препроцессора **CREOBIT_BACKEND_APPSTORE**.

## Инициализация
```csharp
var playFabStore = new PlayFabStore(catalogVersion: null, storeId: null)
{
    CurrencyMap = new List<(string CurrencyId, string VirtualCurrency)>
    {
        ( "Money", "RM" ),
        ( "Coins", "CC" )
    },
    ProductMap = new List<(string ProductId, string ItemId)>
    {
        ("AppBox", "PlayFabBox"),
        ("AppKey", "PlayFabKey")
    }
};

var appStorePlayFabStore = new AppStorePlayFabStore(playFabStore)
{
    ProductMap = new List<(string ProductId, ProductType ProductType)>
    {
        ("GooglePlayBox", ProductType.Consumable),
        ("GooglePlayKey", ProductType.Consumable)
    }
};
```

# Creobit.Backend.Core
Набор базовых интерфейсов для работы с бэкендом.

## Установка
Не требуется.

## Основные интерфейсы

### IApplicationData
Интерфейс для работы с данными приложения.

```csharp
// Прочитать данные.
void Read<T>(Action<T> onComplete, Action onFailure);
```

### IAuth
Интерфейс аутентификации.

```csharp
// Выполнен вход.
bool IsLoggedIn { get; }

// Выполнить вход.
void Login(Action onComplete, Action onFailure);

// Выполнить выход.
void Logout(Action onComplete, Action onFailure);
```

### IExceptionHandler
Интерфейс обработки исключений.

```csharp
// Обработать исключение.
void Process(Exception exception);
```

### ILink
Интерфейс связывания аккаунтов.

```csharp
// Выполнить линковку.
void Link(string linkKey, Action onComplete, Action onFailure);

// Запросить ключ для линковки.
void RequestLinkKey(int linkKeyLenght, Action<string> onComplete, Action onFailure);
```

### IStore
Интерфейс работы с магазином.

```csharp
// Продукты.
IEnumerable<IProduct> Products { get; }

// Загрузить продукты.
void LoadProducts(Action onComplete, Action onFailure);
```

### IUser
Интерфейс пользователя.

```csharp
// Имя пользователя.
string UserName { get; }

// Установить имя пользователя.
void SetUserName(string userName, Action onComplete, Action onFailure);
```

### IUserData
Интерфейс для работы с данными пользователя.

```csharp
// Прочитать данные.
void Read<T>(Action<T> onComplete, Action onFailure) where T : class, new();

// Записать данные.
void Write(object data, Action onComplete, Action onFailure);
```

## Вспомогательные интерфейсы

### IProduct
Интерфейс продукта.

```csharp
// Внутренний набор валют.
IEnumerable<(string CurrencyId, uint Count)> BundledCurrencies { get; }

// Внутренний набор продуктов.
IEnumerable<(IProduct Product, uint Count)> BundledProducts { get; }

// Описание продукта.
string Description { get; }

// Идентификатор продукта.
string Id { get; }

// Имя продукта.
string Name { get; }

// Цены.
IEnumerable<(string CurrencyId, uint Price, string CurrencyCode)> Prices { get; }

// Выполнить покупку.
void Purchase(string currencyId, Action onComplete, Action onFailure);
```

## Расширения

TODO

# Creobit.Backend.CustomPlayFab
Модуль для работы с **PlayFab** посредством **CustomId**.

## Установка
* Выполнить шаги для установки [**Creobit.Backend.PlayFab**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendplayfab).

## Инициализация
```csharp
var playFabAuth = new PlayFabAuth(titleId: "");
var playFabLink = new PlayFabLink();

var customPlayFabAuth = new CustomPlayFabAuth(playFabAuth, customId: "");
var customPlayFabLink = new CustomPlayFabLink(playFabLink, customPlayFabAuth);
```

# Creobit.Backend.GooglePlay
Модуль для работы с **GooglePlay**.

## Установка
* Импортировать пакет **[Assets/Creobit/Backend/GooglePlay/Packages/GooglePlayGamesPlugin-0.9.64.unitypackage](https://github.com/Creobit-Ltd/Creobit.Backend/blob/master/GooglePlay/Packages/GooglePlayGamesPlugin-0.9.64.unitypackage)** и выполнить [шаги для установки](https://github.com/playgameservices/play-games-plugin-for-unity);
* Импортировать пакет **[Assets/Creobit/Backend/GooglePlay/Packages/Injection.unitypackage](https://github.com/Creobit-Ltd/Creobit.Backend/blob/master/GooglePlay/Packages/Injection.unitypackage)**;
* Добавить директиву препроцессора **CREOBIT_BACKEND_GOOGLEPLAY**.

## Инициализация
```csharp
var googlePlayAuth = new GooglePlayAuth();
var googlePlayUser = new GooglePlayUser();
```

# Creobit.Backend.GooglePlayPlayFab
Модуль для работы с **PlayFab** посредством **GooglePlay**.

## Установка
* Выполнить шаги для установки [**Creobit.Backend.GooglePlay**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendgoogleplay);
* Выполнить шаги для установки [**Creobit.Backend.PlayFab**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendplayfab);
* Выполнить шаги для установки [**UnityIAP**](https://docs.unity3d.com/Manual/UnityIAPSettingUp.html).

## Инициализация
```csharp
var playFabAuth = new PlayFabAuth(titleId: "");
var playFabLink = new PlayFabLink();
var playFabStore = new PlayFabStore(catalogVersion: null, storeId: null)
{
    CurrencyMap = new List<(string CurrencyId, string VirtualCurrency)>
    {
        ( "Money", "RM" ),
        ( "Coins", "CC" )
    },
    ProductMap = new List<(string ProductId, string ItemId)>
    {
        ("AppBox", "PlayFabBox"),
        ("AppKey", "PlayFabKey")
    }
};
var playFabUser = new PlayFabUser(playFabAuth);

var googlePlayAuth = new GooglePlayAuth();
var googlePlayUser = new GooglePlayUser();

var googlePlayPlayFabAuth = new GooglePlayPlayFabAuth(playFabAuth, googlePlayAuth);
var googlePlayPlayFabLink = new GooglePlayPlayFabLink(playFabLink, googlePlayPlayFabAuth);
var googlePlayPlayFabStore = new GooglePlayPlayFabStore(playFabStore, publicKey: "")
{
    ProductMap = new List<(string ProductId, ProductType ProductType)>
    {
        ("GooglePlayBox", ProductType.Consumable),
        ("GooglePlayKey", ProductType.Consumable)
    }
};
var googlePlayPlayFabUser = new GooglePlayPlayFabUser(playFabUser, googlePlayUser);
```

**GooglePlayPlayFabStore.ProductMap** - содержит список продуктов *GooglePlay* (Ключ продукта / Тип продукта). Ключ продукта должен совпадать с идентификатором предмета *PlayFab* для возможности валидации.

# Creobit.Backend.IosPlayFab
Модуль для работы с **PlayFab** посредством **iOS**.

## Установка
* Выполнить шаги для установки [**Creobit.Backend.PlayFab**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendplayfab);
* Добавить директиву препроцессора **CREOBIT_BACKEND_IOS**.

## Инициализация
```csharp
var playFabAuth = new PlayFabAuth(titleId: "");
var playFabLink = new PlayFabLink();

var iosPlayFabAuth = new IosPlayFabAuth(playFabAuth);
var iosPlayFabLink = new IosPlayFabLink(playFabLink, iosPlayFabAuth);
```

# Creobit.Backend.PlayFab
Модуль для работы с **PlayFab**.

## Установка
* Импортировать пакет **[Assets/Creobit/Backend/PlayFab/Packages/UnitySDK.unitypackage](https://github.com/Creobit-Ltd/Creobit.Backend/blob/master/PlayFab/Packages/UnitySDK.unitypackage)** и выполнить [шаги для установки](https://api.playfab.com/sdks/unity);
* Импортировать пакет **[Assets/Creobit/Backend/PlayFab/Packages/Injection.unitypackage](https://github.com/Creobit-Ltd/Creobit.Backend/blob/master/PlayFab/Packages/Injection.unitypackage)**;
* Добавить директиву препроцессора **CREOBIT_BACKEND_PLAYFAB**.

## Инициализация
```csharp
var playFabApplicationData = new PlayFabApplicationData();
var playFabAuth = new PlayFabAuth(titleId: "");
var playFabLink = new PlayFabLink();
var playFabStore = new PlayFabStore(catalogVersion: null, storeId: null)
{
    CurrencyMap = new List<(string CurrencyId, string VirtualCurrency)>
    {
        ( "Money", "RM" ),
        ( "Coins", "CC" )
    },
    ProductMap = new List<(string ProductId, string ItemId)>
    {
        ("AppBox", "PlayFabBox"),
        ("AppKey", "PlayFabKey")
    }
};
var playFabUser = new PlayFabUser(playFabAuth);
var playFabUserData = new PlayFabUserData();
```

**PlayFabStore.CurrencyMap** - содержит список соответствия ключей валют (Ключ валюты приложения / Идентификатор валюты *PlayFab*).

**PlayFabStore.ProductMap** - содержит список соответствия ключей продуктов (Ключ продукта приложения / Идентификатор предмета *PlayFab*).

## Вспомогательные интерфейсы

### IPlayFabErrorHandler
Интерфейс обработки ошибок *PlayFab*.

```csharp
// Обработать ошибку.
void Process(PlayFabError playFabError);
```

## Расширения

TODO

# Creobit.Backend.Steam
Модуль для работы со **Steam**.

## Установка
* Импортировать пакет **[Assets/Creobit/Backend/Steam/Packages/Facepunch.Steamworks.unitypackage](https://github.com/Creobit-Ltd/Creobit.Backend/blob/master/Steam/Packages/Facepunch.Steamworks.unitypackage)** и выполнить [шаги для установки](https://github.com/Facepunch/Facepunch.Steamworks);
* Добавить директиву препроцессора **CREOBIT_BACKEND_STEAM**.

## Инициализация
```csharp
var steamAuth = new SteamAuth(appId: 0);
var steamUser = new SteamUser();
```

# Creobit.Backend.SteamPlayFab
Модуль для работы с **PlayFab** посредством **Steam**.

## Установка
* Выполнить шаги для установки [**Creobit.Backend.PlayFab**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendplayfab);
* Выполнить шаги для установки [**Creobit.Backend.Steam**](https://github.com/Creobit-Ltd/Creobit.Backend#creobitbackendsteam).

## Инициализация
```csharp
var playFabAuth = new PlayFabAuth(titleId: "");
var playFabLink = new PlayFabLink();
var playFabStore = new PlayFabStore(catalogVersion: null, storeId: null)
{
    CurrencyMap = new List<(string CurrencyId, string VirtualCurrency)>
    {
        ( "Money", "RM" ),
        ( "Coins", "CC" )
    },
    ProductMap = new List<(string ProductId, string ItemId)>
    {
        ("AppBox", "PlayFabBox"),
        ("AppKey", "PlayFabKey")
    }
};
var playFabUser = new PlayFabUser(playFabAuth: playFabAuth);

var steamAuth = new SteamAuth(appId: 0);
var steamUser = new SteamUser();

var steamPlayFabAuth = new SteamPlayFabAuth(playFabAuth, steamAuth);
var steamPlayFabLink = new SteamPlayFabLink(playFabLink, steamPlayFabAuth);
var steamPlayFabStore = new SteamPlayFabStore(playFabStore);
var steamPlayFabUser = new SteamPlayFabUser(playFabUser, steamUser);
```

# Лизензия
[MIT](https://github.com/Creobit-Ltd/Creobit.Backend/blob/master/LICENSE.md)
