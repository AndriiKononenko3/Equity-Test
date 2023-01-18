open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Settings
open CompositionRoot
open Api
open Equity.Api.CompositionRoot

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configureApp (compositionRoot: CompositionRoot) (app: IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()

    (match env.EnvironmentName with
     | "Development" -> app.UseDeveloperExceptionPage()
     | _ -> app.UseGiraffeErrorHandler(errorHandler))
        .UseHttpsRedirection()
        .UseStaticFiles()
        .UseGiraffe(HttpHandler.router compositionRoot)

let configureServices (configurationBuilder: IConfigurationBuilder)(services: IServiceCollection) =
    services
        .Configure<IdGenerator.Settings>(configurationBuilder.Build().GetSection("IdGeneratorSettings"))
        .AddLogging()
        .AddGiraffe() |> ignore

let configureLogging (builder: ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Error).AddConsole().AddDebug()
    |> ignore

let configureSettings (configurationBuilder: IConfigurationBuilder) =
    configurationBuilder
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", false)

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")
    let confBuilder = ConfigurationBuilder() |> configureSettings
    let trunk = Trunk.compose (confBuilder.Build().Get<Settings>())
    let root = CompositionRoot.compose trunk

    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .UseContentRoot(contentRoot)
                .UseWebRoot(webRoot)
                .UseUrls("http://localhost:5221")
                .Configure(Action<IApplicationBuilder>(configureApp root))
                .ConfigureServices(configureServices confBuilder)
                .ConfigureLogging(configureLogging)
            |> ignore)
        .Build()
        .Run()

    0
