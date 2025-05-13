# Alga.wwwcore

Alga.wwwcore nuget package - ASP NET Frontend Web Server, for JavaScript Developers. Позволяет сделать ваш Web Site быстрее - использует бандлинг и минимификацию и другие инструменты копресссии и кеширования, позволяет работать вашему проекту offline благодаря ServiceWorker.js. Можно использовать веб как приложение благодаря PWA, если добавить на экран устройства. Быстрое создание Android App, на базе PWA (MPA)

optimizes Web development if you write all the code in Javascript. You get a fast Web app, with the ability to work offline (PWA) and the ability to quickly turn it into an Android App


- Less UI logic in asp.net
- Automatically generate a `<head>` for the correct operation of the web application
- The "bindleconfig.json" file is automatically generated, which defines the rules for bindling and minifying javascript & css files for each page (UI Screen)
- Automatically generated "manifest.json" wНhich is necessary for PWA to work correctly
- Tool to add SEO instructions for each page (user interface screen) and the ability to override them

- Автоматически создается правильно работающий PWA который может работать как online так и offline (полноценное приложение которое работает на движке браузера) и вы можете использовать его как полноценное приложение добавыа иконку на экран приложения

- Android App - для того чтобы создать на то чтобы создать полноценное андроид приложение ва нужно выполнить несколько комманд и у вас уже будет полноценный apk файл




## How does this work. Step by step


1. Create ASP.NET Core project or open an existing project

2. Add [Alga.wwwcore](https://www.nuget.org/packages/Alga.wwwcore) nuget package

`> dotnet add package Alga.wwwcore --version X.X.X` (check last version)

3. НАСТРОЙКА


4. Open "wwwroot" directory (if not exist - create it)

- Create "Components" dirictory (wwwroot/Modules)
- - Create "Total" dirictory - is main component
- - - Create "content" dirictory (wwwroot/Modules/Total/content) - this is where images and other files will be stored
- - - - Add Icons: Icon-32.png (32x32px), "Icon-180.png" (180x180px), "Icon-192.png" (192x192px), "Icon-512.png" (512x512px)
- - - - Add screenshots: screenshot-vertical.png (1080x1920px), screenshot-horizontal.png (1920x1080px)
- Create "UIRs" dirictory (wwwroot/UIRs) - For your Pages (UI Screens) - Required
- - Create Page (UI Screen) directory {Index} (wwwroot/UIRs/Index)
- - - Create file "script.js" (wwwroot/UIRs/Index/script.js) - Required. This is the file that will be called to build the page (UI Screen)
- - - Create file "style.css" (wwwroot/UIRs/Index/script.js) - File for storing styles
- - - Create "content" directory (wwwroot/UIRs/Index/content) - Вirectory for storing images and other page's content
- - - Create file "scheme.json" (wwwroot/UIRs/Index/scheme.json) - Page (UI Screen) scheme with primary information about the page and a list of links to external components (modules) that are used on this page (called script.js)

6. Add in "appsettings.json"

```
{
    ...
    "AlgaWwwcoreConfig": {
        "Name": "My Awesome App",
        "NameShort": "AwesomeApp",
        "Description": "A great app for managing your tasks.",
        "CacheControlInSDefault": 10800,
        "PreconnectUrls": ["https://api.example.com", "https://api1.example.com", "https://api2.example.com"],
        "GoogleFontsUrl": "https://fonts.googleapis.com/css2?family=Audiowide&family=Montserrat:wght@500;600;700&family=Nunito:wght@500;700&Mulish:wght@500&display=swap",
        "GoogleAnalyticsCode": "G-2233KK2222",
        "YandexMetrikaCode": "88888888",
        "BackgroundColor": "#1f1f1f",
        "ThemeColor": "#1f1f1f",
        "Lang": "en"
    }
    ...
}
```

CacheControlInSDefault  - The default cache management duration (in seconds) for all application UI Screens. The default value is -1, indicating no cache control by default.

ЗАПУСК

6. Program.cs & "Alga.wwwcore" nuget package

Устанавливаете ссылку на пакет 

```
using Alga.wwwcore;
```

Устанавливаете AddHttpContextAccessor - это важно

```
builder.Services.AddHttpContextAccessor();
```

Парсим и встраиваем в код иннформмацию из конигурационного файла

```
builder.Services.Configure<Root.ConfigModel>(builder.Configuration.GetSection("AlgaWwwcoreConfig"));
```

Пример конфогурационного файла appsettings.json, то есть то что можно установить один раз и забыть 

```
{
    ...
  "AlgaWwwcoreConfig": {
    "Name": "RT.Ink",
    "NameShort": "RT - Your Feed",
    "Description": "Discover breaking news and stay informed with RT.ink. Explore global headlines, in-depth analysis, and top stories tailored for you. Join our platform today!",
    "CacheControlInSDefault": 10800, 
    "PreconnectUrls": ["https://api.rt.com"],
    "GoogleFontsUrl": "https://fonts.googleapis.com/css2?family=Audiowide&family=Montserrat:wght@500;600;700&family=Nunito:wght@500;700&Mulish:wght@500&display=swap",
    "BackgroundColor": "#1f1f1f",
    "ThemeColor": "#1f1f1f",
    "Lang": "en"
    "schemes": [
    {
        "title": "Bookmarks - RT.ink",
        "description": "Your saved articles collection",
        "robot": "noindex",
        "modules": []
    },
    {
        "title": "Trending News - RT.ink",
        "description": "Most popular news right now",
        "robot": "index,follow",
        "modules": []
    }
    ]
  }
    ...
}

```

schemes: здесь находится информация о структуре ваших страниц, 

Запускаем ваш Alga.wwwcore, он позволяет комфортно работать в DEV режиме, и работать со струектурой кода так как вы ее видите. И в Release режиме, при каждом запуке в режиме DEBUG генерируется оптиизированный код который будет быстрее работать на сервере, то есть примменяется минификация, бандификация и прочиее поджодыю. Так же подготавливается PWA который позволит вам в последующем довольно легко превратить в Androin & iOS app. (SPA?)

```
builder.Services.AddSingleton(sp => new Root(sp.GetRequiredService<IOptions<Root.ConfigModel>>().Value, sp.GetRequiredService<IHttpContextAccessor>()));
```

Примеры вызова В asp core minial api

```

AppMapGet("/", "i");
app.MapGet("/a/{urlShort}", async (Root www, IHttpClientFactory httpClientFactory) => await www.SendAsync("i", await i_seo(www.HttpContextAccessor.HttpContext, httpClientFactory, memoryCache)));
AppMapGet("/bookmarks", "bookmarks");

void AppMapGet(string route, string template) => app.MapGet(route, (Root www) => www.SendAsync(template));

```




5. About scheme.json

```
{
    "title": "Main page",
    "description": "About Main page",
    "robot": "noindex",
    "modules": [ 
        { "path": "/Modules/ApiRtInk_MSAP", "toBundlerMinifier": true },
        { "path": "/Modules/HeadHtmlBox", "toBundlerMinifier": true },
        { "path": "/Modules/HeaderTextUI", "toBundlerMinifier": true },
        { "path": "/Modules/FooterInfHtmlBox", "toBundlerMinifier": true },
        { "path": "/Modules/ListGHtmlBox", "toBundlerMinifier": true },
        { "path": "/Modules/MoreButtonUI", "toBundlerMinifier": true }
        { "path": "/ExternalComponents/MomentjsCom/moment-timezone.min.js" }
    ]
}
```

title - The title (tag) of page that will be used in HEAD if not overridden
description - The description (meta) of page that will be used in HEAD if not overridden
robot - The <meta name="robots"> tag is an HTML element that provides instructions to search engine crawlers (or robots) about how to index and follow the links on a webpage.
modules - List of paths to components that will be called by the script.js of page (UI Screen).This list also helps determine which components should be bundled & minify with script & css pages. If you do not want to specify the path to all files, specify the path to the directory and we will get the paths to javascript & css files





### Publication

Terminal command: ```dotnet publish -c Release -r win-x64 --force```





### Create Android APP (TWA) using Bubblewrap

Install bubblewrap: ```npm install -g @bubblewrap/cli```

```bubblewrap build --clean```

```keytool -genkeypair -v \
  -keystore "/Users/xxx/Documents/Mask/Projects/RtInkGit/android.keystore" \
  -alias android \
  -keyalg RSA -keysize 2048 \
  -validity 10000 \
  -storepass "YourPass" \
  -keypass "YourPass"```

```bubblewrap update \
  --keystore=/Users/xxx/Documents/Mask/Projects/RtInkGit/android.keystore \
  --ks-pass="pass:YourPass" \
  --key-pass="pass:YourPass"```


```bubblewrap update --keystore=./android.keystore```

```bubblewrap init --manifest https://your_site.com/manifest.json```

```bubblewrap build```

bubblewrap validate

curl -I https://rt.ink/manifest.json


Do you want Bubblewrap to install the JDK: Y (Yes)

Do you want Bubblewrap to install the Android SDK: Y (Yes)

------- ink.rt.twa





### Logging

A logging system with hints and error information was added to the project. Monitor them in the debug console.





### Upates

What has been changed in new build (4.0.0) compared to the previous version (3.2.3)

- NEW: Added the ability to obfuscate JavaScript code

- Удален BuildBundlerMinifier - плохо справляется с мминификацией на проектах с современным кодом. Добавлена собственная реализация Bundler + NUglify
- Перенесли настройку структуры ваших страниц в настройки appsettings.json, что сделало код более гибким, теперь вы в гораздо меньшей степени зависите ин структуры расположения кода навязанной нами 
- Добавлена возможность обфускации JS кода




## ASP.NET Core Project

An example of using Alga.wwwcore nuget package on a real ASP.NET Core project

web: [https://git.rt.ink](https://git.rt.ink)
git: [https://github.com/rtink-git/RtInkGit](https://github.com/rtink-git/RtInkGit/tree/main/RtInkGit/RtInkGit)





## Additional settings -  которые вам должны помочь

1. Настройка .csproj - проекта asp где находятся ваши script & style файлы цель не отправлять файлы на сервер которые не будут на нем применяться и служат или для сборки проекта или в dev версии

``` 
<ItemGroup>
    <Content Update="wwwroot/UISs/**/script.js" CopyToPublishDirectory="Never" />
    <Content Update="wwwroot/UISs/**/style.css" CopyToPublishDirectory="Never" />
    <Content Update="wwwroot/UISs/**/scheme.json" CopyToPublishDirectory="Never" />
</ItemGroup>

```





## Telegram mini app

```
// Формируем клавиатуру с кнопкой WebApp - в телеграм боте

var requestData = new 
{
    chat_id = chatId,
    text = "Нажмите кнопку, чтобы открыть Mini App!",
    reply_markup = new
    {
        inline_keyboard = new[]
        {
            new[]
            {
                new
                {
                    text = "Open Mini App",
                    web_app = new { url = webAppUrl }
                }
            }
        }
    }
};
```

```
// Формируем клавиатуру с кнопкой WebApp - канал

var requestData = new 
{
    chat_id = chatId,
    text = "Нажмите кнопку, чтобы открыть Mini App!",
    reply_markup = new
    {
        inline_keyboard = new[]
        {
            new[]
            {
                new
                {
                    text = "Open Mini App",
                    url = "https://t.me/your_bot?startapp"
                }
            }
        }
    }
};
```