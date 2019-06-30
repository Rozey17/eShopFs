module eShop.App

open System
open System.IO
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open eShop.Domain.ConferenceManagement.Conference.CreateConference.Web
open eShop.Domain.ConferencePublic.DisplayConference.Web

[<AutoOpen>]
module Middleware =

    open Microsoft.AspNetCore.Mvc.Razor

    type IServiceCollection with
        member this.AddRazorEngine =
            this.Configure<RazorViewEngineOptions>(
                fun (options : RazorViewEngineOptions) ->
                    options.ViewLocationFormats.Clear()
                    options.ViewLocationFormats.Add("/Domain.ConferenceManagement.Conference/{1}/{0}.cshtml")
                    options.ViewLocationFormats.Add("/Domain.ConferencePublic/{1}/{0}.cshtml")
                )
                .AddMvc()
            |> ignore
            this.AddAntiforgery()


// ---------------------------------
// Models
// ---------------------------------

[<CLIMutable>]
type Message =
    {
        Text : string
    }

let handleGetHello =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let response = {
                Text = "Hello world, from Giraffe 2!"
            }
            return! json response next ctx
        }

// ---------------------------------
// Web app
// ---------------------------------

let webApp =
    choose [
        GET >=>
            choose [
                route  "/conferences/create" >=> renderCreateReferenceView

                // TODO: change route
                route  "/conferences/display" >=> renderDisplayConferenceView
            ]
        POST >=>
            choose [
            ]
        text "Not Found" |> RequestErrors.notFound ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffeErrorHandler(errorHandler)
       .UseStaticFiles()
       .UseMvc()
       .UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddRazorEngine |> ignore


let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")

    WebHostBuilder()
        .UseKestrel()
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0