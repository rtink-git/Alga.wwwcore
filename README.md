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


### 1. Create ASP.NET Core project or open an existing project

### 2. Add [Alga.wwwcore](https://www.nuget.org/packages/Alga.wwwcore) nuget package

### 3. Optimize ASP.NET Core for Maximum Performance: Configure Kestrel, Rate Limiter, Caching, and Security. (Program.cs)

``` 
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

var isDebug = builder.Environment.IsDevelopment();





// Kestrel Server Configuration
// -------------------------------
// Production-optimized web server setup with security hardening, protocol optimization and connection management for high-load scenarios

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Limit the maximum size of the request body to 32 KB.
    // Useful for static sites or APIs that only receive small payloads (like query parameters, not file uploads).
    serverOptions.Limits.MaxRequestBodySize = 32 * 1024; // 32 KB

    // Reduce the TCP keep-alive timeout.
    // This limits how long idle connections stay open, freeing up resources.
    // A short timeout like 5 seconds is ideal for static websites, SPAs, or CDN-like behavior.
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(5);

    // Set a strict timeout for receiving request headers.
    // Helps mitigate slowloris-type attacks and ensures fast failure for broken clients.
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(5);

    // Disable the default "Server: Kestrel" response header.
    // This improves security by not disclosing the server type and saves a few bytes per response.
    serverOptions.AddServerHeader = false;

    // Set a cap on the number of concurrent TCP connections handled by Kestrel.
    // This value can be tuned based on the server's capabilities and expected traffic.
    serverOptions.Limits.MaxConcurrentConnections = 10_000;

    // Specify supported HTTP protocols for endpoints (applies to all by default).
    // HTTP/2 allows multiplexing multiple streams over a single connection — useful for static sites and APIs.
    // HTTP/1 is kept for backward compatibility.
    serverOptions.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    // Optional: If you want full control over HTTPS, ports, and protocols, define explicit listeners.
    // Example below sets up HTTPS on port 443 with specified protocols.
    /*
    serverOptions.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps(); // TLS is required for HTTP/2 and HTTP/3
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    */
});





// Global Request Throttling
// -------------------------
// Per-client (IP) token bucket limiter to enforce request rate caps, prevent DoS-like behavior, and maintain service responsiveness under load.

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            // Пропускаем health-чекеры
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return RateLimitPartition.GetNoLimiter("healthchecks");

            // Reject requests with missing IP address
            var ip = httpContext.Connection.RemoteIpAddress?.ToString();
            if (ip is null)
            {
                httpContext.Response.StatusCode = 400;
                return RateLimitPartition.GetNoLimiter("invalid-ip");
            }

            return RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: ip,
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    // Maximum number of tokens available at once (burst capacity).
                    // Allows short spikes of traffic.
                    TokenLimit = isDebug ? 600 : 200,

                    // Tokens added per replenishment cycle (sustained rate).
                    TokensPerPeriod = 20, // Up to 20 requests per second

                    // Interval at which tokens are refilled.
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),

                    // How many requests can be queued when tokens run out.
                    QueueLimit = 20, // Allows some short waiting

                    // Requests are processed in FIFO order from the queue.
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,

                    // Tokens are refilled automatically by a background timer.
                    AutoReplenishment = true
                });
        }),
        
        // Global sliding window limiter (to protect backend globally)
        PartitionedRateLimiter.Create<HttpContext, string>(_ =>
            RateLimitPartition.GetSlidingWindowLimiter(
                "GlobalLimit",
                _ => new SlidingWindowRateLimiterOptions
                {
                    // Total allowed requests per window
                    PermitLimit = 5000,
                    // Time window duration
                    Window = TimeSpan.FromSeconds(1),
                    // Subdivision of window (e.g. 2 × 500ms)
                    SegmentsPerWindow = 2
                }))
    );

    // Respond with HTTP 429 "Too Many Requests" when rate limit is exceeded.
    options.RejectionStatusCode = 429;

    // Optional: Set a Retry-After header to suggest when the client can retry.
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.Headers["Retry-After"] = "10";
        await ValueTask.CompletedTask;
    };
});





// Global Response Compression
// ---------------------------
// Global response compression setup to reduce response size over HTTPS and improve overall performance for supported MIME types.

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Clear(); // Remove default providers (e.g., Gzip)
    options.Providers.Add<BrotliCompressionProvider>(); // Use Brotli compression (modern, efficient)
    options.EnableForHttps = true; // Enable compression for HTTPS responses

    // List of MIME types to compress
    options.MimeTypes = new[]
    {
        "text/*",                      // HTML, CSS, plain text, etc.
        "application/*+xml",          // XML-based formats (e.g. SVG, Atom)
        "application/json",           // JSON responses
        "application/javascript",     // JS files
        "application/wasm",           // WebAssembly modules
        "application/octet-stream",   // Binary data (e.g. Blazor DLLs)
        "image/svg+xml",              // SVG images
        "application/x-msgpack",      // MessagePack (binary JSON)
        "font/woff2"                  // Compressed web fonts
    };
});

// Configure Brotli compression level
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest; // Use fastest (least CPU-intensive) compression
});





// Global Output Cache Policy
// --------------------------
// Configures output caching to improve performance and reduce response time for repeat requests.
// Includes a named policy with server and client cache control for specific endpoints.

var HOutputCachePolicy = "HOutputCachePolicy";
builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.Zero;
    options.AddPolicy(HOutputCachePolicy, builder => builder.Expire(TimeSpan.FromSeconds(RtInk.Constants.HInSecForCache)));
});





// Global Request Timeout Policy
// -----------------------------
// Global request timeout setup to limit request execution time and improve service reliability.
// Defines a default timeout and a named short-timeout policy for specific endpoints.

var S5TimeoutPolicy = "S5TimeoutPolicy";
builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(3), TimeoutStatusCode = 503 };
    options.AddPolicy(S5TimeoutPolicy, TimeSpan.FromSeconds(5));
});





// HTTP Strict Transport Security (HSTS)
// -------------------------------------
// Configures HSTS to enforce HTTPS by instructing browsers to always use secure connections.
// Helps protect against protocol downgrade attacks and cookie hijacking.

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(720);         // Informs browsers to remember HTTPS enforcement for 720 days
    options.IncludeSubDomains = true;                // Applies HSTS policy to all subdomains as well
    options.Preload = true;                          // Indicates intent to be included in browser preload lists (requires submission)
    
    // Additional configuration:
    options.ExcludedHosts.Add("localhost");          // Excludes localhost to avoid HSTS issues during local development
});





var app = builder.Build();





// Production Error Handling & Security
// ------------------------------------
// In production environments, configure global error handling and enable HSTS
// to enforce HTTPS usage and protect against downgrade attacks.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error"); // Redirects unhandled exceptions to a custom error endpoint
    app.UseHsts();                     // Adds HTTP Strict Transport Security header to enforce HTTPS
    app.UseHttpsRedirection();         // Enable only in Production
}





// Alga.wwwcore - nuget package
// ------------------------------------
// ------------------------------------

var config = builder.Configuration.GetSection("AlgaWwwcoreConfig").Get<Alga.wwwcore.Root.ConfigModel>();
var logger = app.Services.GetRequiredService<ILogger<Alga.wwwcore.Root>>();
if (config != null) await new Alga.wwwcore.Root(config, logger).Start();





// Enables response compression to reduce bandwidth
app.UseResponseCompression();

// Enforces request rate limits to prevent DDoS/abuse
app.UseRateLimiter();

// Automatically cancels long-running requests after timeout
app.UseRequestTimeouts();

// Caches responses to improve performance
app.UseOutputCache();





// Static File Serving with Advanced Caching
// -----------------------------------------
// Configures custom static file handling with fine-tuned cache control, ETag/Last-Modified validation,
// preload hints for critical assets, MIME overrides, and cross-origin protection. Enhances performance,
// reduces redundant transfers, and improves user experience across browsers and CDNs.

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var headers = ctx.Context.Response.Headers;
        var file = ctx.File;
        var fileName = file.Name;
        var fileExt = Path.GetExtension(fileName);
        var path = file.PhysicalPath;

        int maxAge = (fileName.ToLowerInvariant()) switch
        {
            "serviceworker.js" => 0,
            "app.js" or "manifest.json" => RtInk.Constants.HInSecForCache,
            _ => fileExt.ToLowerInvariant() switch
            {
                ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp" => RtInk.Constants.ThirtyDInSecForCache,
                ".css" or ".js" or ".html" => RtInk.Constants.HInSecForCache,
                _ => RtInk.Constants.HInSecForCache
            }
        };

        bool isImmutable = maxAge > 0;

        headers["Cache-Control"] = $"public, max-age={maxAge}" + (isImmutable ? ", immutable" : ", must-revalidate");
        headers["Vary"] = "Accept-Encoding";
        headers["Cross-Origin-Resource-Policy"] = "same-origin";

        var lastModified = file.LastModified.UtcDateTime;
        string etag = $"\"{lastModified.Ticks:x}\""; // hex ticks

        headers["Last-Modified"] = lastModified.ToString("R"); // RFC1123
        headers["ETag"] = etag;

        var request = ctx.Context.Request;

        // ETag: If-None-Match
        if (request.Headers.TryGetValue("If-None-Match", out var inm) && inm == etag)
        {
            ctx.Context.Response.StatusCode = StatusCodes.Status304NotModified;
            ctx.Context.Response.Body = Stream.Null;
            return;
        }

        // If-Modified-Since (дополнительно к ETag)
        if (request.Headers.TryGetValue("If-Modified-Since", out var ims)
            && DateTime.TryParse(ims, out var imsDate)
            && lastModified <= imsDate)
        {
            ctx.Context.Response.StatusCode = StatusCodes.Status304NotModified;
            ctx.Context.Response.Body = Stream.Null;
            return;
        }

        // HTTP/103 Early Hints
        if (ctx.Context.Response.SupportsTrailers())
        {
            var preloadType = fileExt switch
            {
                ".js" => "script",
                ".css" => "style",
                ".woff2" => "font",
                _ => null
            };
            if (preloadType != null)
            {
                headers.Append("Link", $"</{file.Name}>; rel=preload; as={preloadType}");
            }
        }

        // Custom content types (если нужно)
        switch (fileName.ToLowerInvariant())
        {
            case "manifest.json":
                headers["Content-Type"] = "application/manifest+json";
                break;
            case "serviceworker.js":
                headers["Content-Type"] = "application/javascript";
                break;
        }
    }
});





// Global Security Headers
// ------------------------
// Sets core HTTP security headers to improve privacy, prevent clickjacking, avoid MIME sniffing,
// and restrict access to sensitive browser features like camera, microphone, and opener context.

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    // Prevents MIME type sniffing (e.g. executing images as scripts)
    headers["X-Content-Type-Options"] = "nosniff";

    // Denies page embedding via <iframe>, <frame>, or <object> (clickjacking protection)
    headers["X-Frame-Options"] = "DENY";

    // Ensures no referrer information is sent with requests (maximizes privacy)
    headers["Referrer-Policy"] = "no-referrer";

    // Restricts use of sensitive browser APIs from this origin (hardens access to device features)
    headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

    // Isolates the browsing context but still allows popups to retain opener (useful for SPA+MPA apps)
    headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";

    await next();
});





await app.RunAsync();
``` 

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

- Removed BuildBundlerMinifier - deprecated. Added simplified and modern Bundler implementation + NUglify
- General project settings are now recommended to be stored in appsettings.json
- Now the application architecture can be designed more flexibly. Now it works like this: we look for directories where there is a scheme.json and consider that the files contained in them form the pages of the web application (user interface screen). Important: The names of the directories that contain scheme.json are unique names of your pages, which should not be repeated.
- Added additional information to the documentation about setting up your ASP.NET that may be useful
- SEO optimization tools have been returned
- Changed the logic of building and storing http document, now its formation and transmission are carried out faster





## ASP.NET Core Project

An example of using Alga.wwwcore nuget package on a real ASP.NET Core project

web: [https://git.rt.ink](https://git.rt.ink)
git: [https://github.com/rtink-git/RtInkGit](https://github.com/rtink-git/RtInkGit/tree/main/RtInkGit/RtInkGit)





## Additional settings

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