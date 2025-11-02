# Alga.wwwcore

Alga.wwwcore is a specialized package for ASP.NET Core, designed to prepare the frontend of web applications for production deployment with support for PWA, Android, and Telegram Mini Apps.
The library operates based on a predefined project structure and ensures its support during development. It automates key stages of frontend preparation, allowing you to focus on application logic rather than infrastructure.

**Description**

The project structure consists of a collection of pages and modules (reusable components) intended for building the user interface.
Each page includes its own JavaScript code, styles, and a schema containing metadata about the page and integration data for modules. Modules contain only scripts and styles and can be reused across multiple pages.

Based on this structure, the library automatically generates SEO-optimized HTML files with required meta tags (<meta>, OpenGraph, JSON-LD, etc.).
All referenced scripts and styles are bundled, minified, and compressed (Gzip, Brotli) into a single file, which is then inlined into the page header to ensure instant interface loading.

The library also auto-generates an optimized ServiceWorker.js, enabling resource caching and partial offline support.

A manifest.json is generated to allow your web project to function as a full-featured Progressive Web App (PWA), including installation to a mobile device’s home screen.

The project is also ready to be packaged into an .apk using Trusted Web Activity (TWA), enabling it to run as a native-like Android application.

When installed as a PWA or launched as an Android app, a “lightweight” SPA mode is activated: navigation between pages occurs without full reloads, delivering behavior close to native apps.

Additionally, the library provides built-in tools for integrating with the Telegram Mini App platform, including support for authenticating users via Telegram.





## How does this work. Step by step


### 1. Create ASP.NET Core project or open an existing project

### 2. Add [Alga.wwwcore](https://www.nuget.org/packages/Alga.wwwcore) nuget package

### 3. Optimize ASP.NET Core for Maximum Performance: Configure Kestrel, Rate Limiter, Caching, and Security. (Program.cs)

``` 
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using System.Buffers;




var builder = WebApplication.CreateBuilder(args);

var isDebug = builder.Environment.IsDevelopment();

// -------------------------------

int maxConnections = 500;
int maxRpsPerIp = (int)(maxConnections * 0.6);

// Kestrel Server Configuration
// -------------------------------
// Production-optimized web server setup with security hardening, protocol optimization and connection management for high-load scenarios

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Limit the maximum size of the request body to 8 KB.
    // Useful for static sites or APIs that only receive small payloads (like query parameters, not file uploads).
    serverOptions.Limits.MaxRequestBodySize = 8 * 1024;

    // Limit the maximum size of the response buffer.
    // Useful for controlling the memory usage and ensuring responses are not too large for quick delivery.
    serverOptions.Limits.MaxResponseBufferSize = 256 * 1024; // 256 MB

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
    serverOptions.Limits.MaxConcurrentConnections = maxConnections;

    // Specify supported HTTP protocols for endpoints (applies to all by default).
    // HTTP/2 allows multiplexing multiple streams over a single connection — useful for static sites and APIs.
    // HTTP/1 is kept for backward compatibility.
    serverOptions.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    // Optional: If you want full control over HTTPS, ports, and protocols, define explicit listeners.
    // Example below sets up HTTPS on port 443 with specified protocols.
    // serverOptions.ListenAnyIP(443, listenOptions =>
    // {
    //     listenOptions.UseHttps(); // TLS is required for HTTP/2 and HTTP/3
    //     listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    // });
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

            return RateLimitPartition.GetTokenBucketLimiter( partitionKey: ip, factory: _ => new TokenBucketRateLimiterOptions
            {
                // Maximum number of tokens available at once (burst capacity).
                // Allows short spikes of traffic.
                TokenLimit = maxRpsPerIp,

                // Tokens added per replenishment cycle (sustained rate).
                TokensPerPeriod = Math.Min(maxRpsPerIp, 100),

                // Interval at which tokens are refilled.
                ReplenishmentPeriod = TimeSpan.FromSeconds(1),

                // How many requests can be queued when tokens run out.
                QueueLimit = 20, // Allows some short waiting

                // Requests are processed in FIFO order from the queue.
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,

                // Tokens are refilled automatically by a background timer.
                AutoReplenishment = true,
            });
        }),

        // Global sliding window limiter (to protect backend globally)
        PartitionedRateLimiter.Create<HttpContext, string>(_ =>
            RateLimitPartition.GetSlidingWindowLimiter(
                "GlobalLimit",
                _ => new SlidingWindowRateLimiterOptions
                {
                    // Total allowed requests per window
                    PermitLimit = maxConnections * 3,

                    // Time window duration
                    Window = TimeSpan.FromSeconds(1),

                    // Subdivision of window (e.g. 2 × 500ms)
                    SegmentsPerWindow = 2,

                    QueueLimit = (int)(maxConnections * 0.02) // Без очереди на глобальном уровне
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





// Alga.wwwcore Root Initialization
// --------------------------------
// Registers the Alga.wwwcore.Root as a singleton service.
// Initializes it with configuration from "AlgaWwwcoreConfig", logger instance, and environment mode.
// Enables structured frontend generation and development-time support based on project config.

builder.Services.AddSingleton<Alga.wwwcore.Root>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>().GetSection("AlgaWwwcoreConfig").Get<Alga.wwwcore.Models.Config>();
    var logger  = sp.GetRequiredService<ILogger<Alga.wwwcore.Root>>();
    var isDebug = sp.GetRequiredService<IHostEnvironment>().IsDevelopment();
    return new Alga.wwwcore.Root(cfg, isDebug, logger);
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

var verFileRx = new Regex(@"\.[0-9]{12}\.min\.(js|css)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (!isDebug)
        {
            var headers = ctx.Context.Response.Headers;
            var file = ctx.File;
            var fileName = file.Name;
            var fileExt = Path.GetExtension(fileName).ToLowerInvariant();

            var (maxAge, isImmutable, useStaleWhileRevalidate) = fileName.ToLowerInvariant() switch
            {
                // Версионированные файлы (с хешем/версией в имени)
                _ when verFileRx.IsMatch(fileName) => (31_536_000, true, true), // 365 дней + immutable + SWR

                // Критические файлы
                "serviceworker.js" => (86_400, false, false), // 24 часа в prod
                "app.js" => (172_800, false, false), // 48 часов в prod
                "manifest.json" => (86_400, false, false), // 24 часа

                // Статические ресурсы
                _ when fileExt is ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp" => (2_592_000, true, true), // 30 дней
                _ when fileExt is ".css" or ".js" or ".html" => (604_800, false, true), // 1 неделя в prod
                _ => (86_400, false, false) // 24 часа по умолчанию
            };

            var cacheControl = new StringBuilder($"public, max-age={maxAge}");
            if (isImmutable) cacheControl.Append(", immutable");
            if (useStaleWhileRevalidate) cacheControl.Append($", stale-while-revalidate={maxAge / 2}");
            headers["Cache-Control"] = cacheControl.ToString();

            headers["Vary"] = "Accept-Encoding";
            headers["Cross-Origin-Resource-Policy"] = "same-origin";

            var lastModified = file.LastModified.UtcDateTime;
            var etag = $"\"{lastModified.Ticks:x}\"";
            headers["Last-Modified"] = lastModified.ToString("R");
            headers["ETag"] = etag;

            // Проверка условий 304 Not Modified
            var request = ctx.Context.Request;
            if (request.Headers.IfNoneMatch.Any(v => v == etag) ||
               (request.Headers.IfModifiedSince is { } ifModifiedSince &&
                DateTime.TryParse(ifModifiedSince, out var ifModifiedSinceDate) &&
                lastModified <= ifModifiedSinceDate))
            {
                ctx.Context.Response.StatusCode = StatusCodes.Status304NotModified;
                ctx.Context.Response.Body = Stream.Null;
                return;
            }

            if (ctx.Context.Response.SupportsTrailers())
            {
                var asType = fileExt switch
                {
                    ".js" => "script",
                    ".css" => "style",
                    ".woff2" => "font",
                    _ => null
                };
                if (asType != null)
                    headers.Append("Link", $"</{fileName}>; rel=preload; as={asType}");
            }

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




// -- endpoints: EXAMPLE

app.MapGet("/", async (HttpContext context, IHttpClientFactory httpClientFactory, Alga.wwwcore.Root www) =>
{
    try
    {
        var endp = new ForhouseW.Core.Endpoints._(httpClientFactory, seoHClient);
        string? modelJson = await endp.ModelJson();
        if (modelJson == null) { context.Response.StatusCode = 404; await context.Response.WriteAsync("Model not found or unable to fetch data."); return; }
        WriteMapGet("i", context, www, endp.Seo(), modelJson);
    }
    catch (HttpRequestException httpEx) { context.Response.StatusCode = 503; await context.Response.WriteAsync($"Service unavailable: {httpEx.Message}"); }
    catch (Exception ex) { context.Response.StatusCode = 500; await context.Response.WriteAsync($"Internal server error: {ex.Message}"); }
}).CacheOutput(HOutputCachePolicy);



namespace ForhouseW.Core.Endpoints;

public class _
{
    private HttpClient _client;
    private Models.Category? _model;

    public _(IHttpClientFactory httpClientFactory, string clientName) => _client = httpClientFactory.CreateClient(clientName);

    public async Task<string?> ModelJson()
    {
        var mj = await _client.GetStringAsync($"{ForhouseW.Constants.UrlApi}/WebApp/CategoryInf_inner_json_c?id=0&search=");
        if (mj != null) _model = JsonSerializer.Deserialize<Models.Category>(mj);
        return mj;
    }

    public Alga.wwwcore.Models.Seo? Seo()
    {
        Alga.wwwcore.Helpers.SchemaOrgJsonBuilder? schema = null;
        if (_model != null)
        {
            schema = new Alga.wwwcore.Helpers.SchemaOrgJsonBuilder().WithContext();
            schema.AddArray("@graph",
                new Seo.OrganizationSOrg().GetSchema(),
                new Seo.WebSiteSOrg().GetSchema(),
                new Seo.StoreSOrg().Get(),
                new Alga.wwwcore.Helpers.SchemaOrgJsonBuilder().WithType("Service").WithName("Консультация по подбору строительных материалов").WithDescription("Профессиональная помощь в выборе оптимальных строительных и отделочных материалов для вашего проекта. Мы учитываем бюджет, сроки, экологичность, эксплуатационные характеристики и дизайнерскую концепцию, чтобы вы получили идеальное соотношение цены и качества.").AddNested("provider", i => i.WithId($"{Constants.Url}#organization")).BuildJsonObject(),
                new Seo.CategoryPageSOrg().Get(0, Constants.Name, _model, Constants.Description)
            );
        }

        return new()
        {
            Title = $"{Constants.Name} - Магазин строительных материалов в Набережных Челнах",
            Description = Constants.Description,
            Robot = "index, follow",
            UrlCanonical = Constants.Url[Constants.Url.Length - 1] == '/' ? null : $"/", 
            Path = "/",
            Lang = Constants.Lang,
            TypeOg = "website",
            ImageUrl = Constants.imgPrimary,
            ImageWidth = Constants.imgWidthPrimary,
            ImageHeight = Constants.imgHeightPrimary,
            ImageEncodingFormat = Constants.imgEncodingFormatPrimary,
            SchemaOrgsJson = schema?.ToJson(false)
        };
    }
}

// Endpoints (ussing Alga.wwwcore)
// --------------------------------
// -- ...
// --------------------------------

await app.RunAsync();
``` 


### 4. Alga.wwwcore Root Initialization and Usage Example in ASP.NET Core

**Important:** To let the library determine which page interface to apply, you need to explicitly specify the path to the page directory. For example: /Pages/Home.

You can pass seo parameters using Alga.wwwcore.Models.Seo? seoModel. If the path does not have changeable parameters, we recommend adding seo parameters inside scheme.json and not using Alga.wwwcore.Models.Seo - which speeds up the delivery of the already formed page to the client


```
// Alga.wwwcore Root Initialization
// --------------------------------
// Registers the Alga.wwwcore.Root as a singleton service.
// Initializes it with configuration from "AlgaWwwcoreConfig", logger instance, and environment mode.
// Enables structured frontend generation and development-time support based on project config.

builder.Services.AddSingleton<Alga.wwwcore.Root>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>().GetSection("AlgaWwwcoreConfig").Get<Alga.wwwcore.Models.Config>();
    var logger  = sp.GetRequiredService<ILogger<Alga.wwwcore.Root>>();
    var isDebug = sp.GetRequiredService<IHostEnvironment>().IsDevelopment();
    return new Alga.wwwcore.Root(cfg, isDebug, logger);
});


// Endpoints Example

string CacheControlValue = $"public, max-age={RtInk.Constants.ThreeHInSecForCache}, stale-while-revalidate={RtInk.Constants.ThreeHInSecForCache * 24}";
string UiPrefix = "/UISs/";
byte[] ErrorBytes = "Internal Server Error"u8.ToArray();

app.MapGet("/", async (HttpContext context, IHttpClientFactory httpClientFactory, Alga.wwwcore.Root www) =>
{
    try
    {
        var endp = new ForhouseW.Core.Endpoints._(httpClientFactory, seoHClient);
        string? modelJson = await endp.ModelJson();
        if (modelJson == null) { context.Response.StatusCode = 404; await context.Response.WriteAsync("Model not found or unable to fetch data."); return; }
        WriteMapGet("i", context, www, endp.Seo(), modelJson);
    }
    catch (HttpRequestException httpEx) { context.Response.StatusCode = 503; await context.Response.WriteAsync($"Service unavailable: {httpEx.Message}"); }
    catch (Exception ex) { context.Response.StatusCode = 500; await context.Response.WriteAsync($"Internal server error: {ex.Message}"); }
}).CacheOutput(HOutputCachePolicy);


namespace ForhouseW.Core.Endpoints;

public class _
{
    private HttpClient _client;
    private Models.Category? _model;

    public _(IHttpClientFactory httpClientFactory, string clientName) => _client = httpClientFactory.CreateClient(clientName);

    public async Task<string?> ModelJson()
    {
        var mj = await _client.GetStringAsync($"{ForhouseW.Constants.UrlApi}/WebApp/CategoryInf_inner_json_c?id=0&search=");
        if (mj != null) _model = JsonSerializer.Deserialize<Models.Category>(mj);
        return mj;
    }

    public Alga.wwwcore.Models.Seo? Seo()
    {
        Alga.wwwcore.Helpers.SchemaOrgJsonBuilder? schema = null;
        if (_model != null)
        {
            schema = new Alga.wwwcore.Helpers.SchemaOrgJsonBuilder().WithContext();
            schema.AddArray("@graph",
                new Seo.OrganizationSOrg().GetSchema(),
                new Seo.WebSiteSOrg().GetSchema(),
                new Seo.StoreSOrg().Get(),
                new Alga.wwwcore.Helpers.SchemaOrgJsonBuilder().WithType("Service").WithName("Консультация по подбору строительных материалов").WithDescription("Профессиональная помощь в выборе оптимальных строительных и отделочных материалов для вашего проекта. Мы учитываем бюджет, сроки, экологичность, эксплуатационные характеристики и дизайнерскую концепцию, чтобы вы получили идеальное соотношение цены и качества.").AddNested("provider", i => i.WithId($"{Constants.Url}#organization")).BuildJsonObject(),
                new Seo.CategoryPageSOrg().Get(0, Constants.Name, _model, Constants.Description)
            );
        }

        return new()
        {
            Title = $"{Constants.Name} - Магазин строительных материалов в Набережных Челнах",
            Description = Constants.Description,
            Robot = "index, follow",
            UrlCanonical = Constants.Url[Constants.Url.Length - 1] == '/' ? null : $"/", 
            Path = "/",
            Lang = Constants.Lang,
            TypeOg = "website",
            ImageUrl = Constants.imgPrimary,
            ImageWidth = Constants.imgWidthPrimary,
            ImageHeight = Constants.imgHeightPrimary,
            ImageEncodingFormat = Constants.imgEncodingFormatPrimary,
            SchemaOrgsJson = schema?.ToJson(false)
        };
    }
}
```

### 5. Alga.wwwcore wist https://schema.org

The library provides a way to build schema.org markup for your endpoints using the Alga.wwwcore.Helpers.SchemaOrgJsonBuilder tool.
You can create your markup in any part of your project and pass it to Alga.wwwcore.Models.Seo? Seo(), and then use it in
WriteMapGet("i", context, www, endp.Seo());.


### 6. Add configuration for correct library operation. (appsettings.json)

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
        "cacheUrls" : [ "/error", "/error/crashed", "/locs", "https://api.example.ink/ListOfCountries" ],
        "UseMessagePack": "true",
        "UseTelegram": "true"
    }
    ...
}
```

### 7. Expected Data Structure

You are free to use any architecture and design approaches when building your web application — the Alga.wwwcore library does not impose any restrictions on implementation. However, it only processes project structures and data formats that conform to its expected layout.

**Example of expected structure in the wwwroot/ directory**

wwwroot/
├── Pages/
│   ├── Home/
│   │   ├── scheme.json    # required (page metadata)
│   │   ├── script.js      # required (page logic)
│   │   └── style.css      # optional page styles
│   ├── About/
│   │   ├── scheme.json    # required
│   │   ├── script.js      # required
│   │   └── style.css
│
├── Modules/
│   ├── HeaderBox/
│   │   ├── script.js (module logic invoked by page script)
│   │   └── style.css (module style)
│   ├── ItemListBox/
│   │   ├── script.js
│   │   └── style.css
│   ├── Total/
│   │   └── style.css
│   └── ServerApis/
│       └── script.js

#### Scheme.json

scheme.json is a required file that defines the structure and metadata of a page. It can be located anywhere within the wwwroot directory.
The library automatically scans the entire wwwroot for scheme.json files and uses their locations to identify the directories that contain scripts and styles for application pages. These directories are then treated as individual pages in the application.

Some parameters are used to form SEO tags in the header

```
{
  "title": "DefaultApp - Welcome to Your Starter Page",
  "description": "This is a sample starter page for your web application. Customize it with your own content, components, and branding.",
  "robot": "noindex",
  "path": "/about"
  "modules": [
    "/Modules/LayoutShell",
    "/Modules/UserAuth",
    "/Modules/TimeUtils",
    "/Modules/NavBar",
    "/Modules/PageHeader",
    "/Modules/ContentList",
    "/Modules/LoadMoreButton",
    "/Modules/FooterBasic"
  ]
}
```
**Field Descriptions**

- title: The <title> tag for the page. Used in the HTML <head>. This can be overridden dynamically at runtime if needed.
- description: The <meta name="description"> tag describing the content of the page. Used by search engines and preview snippets.
- robot: The <meta name="robots"> tag, which gives instructions to search engine crawlers about how to index and follow the page.
Common values:
	•	"index" (default behavior)
	•	"noindex" — exclude from indexing
	•	"nofollow" — don’t follow links
	•	or combined: "noindex, nofollow"
- modules: A list of relative paths to module folders or files. Each module may include:
	•	script.js — logic for the component
	•	style.css — styles for the component
If the page does not contain modules, you can skip adding this field.

#### Page script.js

Each page must include a script.js file that contains the logic for rendering and managing the user interface of that page.

**Important:** Each script.js file containing page logic must be wrapped in the following code pattern.
The name of the exported function must be unique and must not conflict with functions from other pages.

```
export async function CurrentDirectoryName() {
    (ui screen logic)
}
```

#### Module script.js

Module code can be written in any form, but it’s strongly recommended to wrap it in a class or namespace to prevent naming collisions and side effects after scripts are bundled together.

### 8. Publication ```dotnet publish -c Release```

### 9. IIS 

IIS Manager > Application Pools > Aadvanced Settings: 
General: Start Mode = AlwaisRunning
Process model: Idle Time-out (minutes) = 0
Recycling: Regular Time Interval (minutes) = 0
Recycling: Private memory limit (KB) = 0
Recycling: Virtual memory limit (KB) = 0
Rapid Fail Protection: Enabled: False





## Additionally

### Create Android APP (TWA) using Bubblewrap

Install bubblewrap: ```npm install -g @bubblewrap/cli```


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



## Upates

What has been changed in new build (4.2.0) compared to the previous version (4.1.0)

- In the new version, SEO optimization has been improved.
A tool for flexible schema.org markup generation and integration into endpoints has been added.
- SPA mode disableds





## ASP.NET Core Project

An example of using Alga.wwwcore nuget package on a real ASP.NET Core project

web: [https://git.rt.ink](https://git.rt.ink)
git: [https://github.com/rtink-git/RtInkGit](https://github.com/rtink-git/RtInkGit)