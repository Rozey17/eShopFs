module eShop.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open eShop.Domain.ConferenceManagement.Home.Web
open eShop.Domain.ConferenceManagement.ConferenceDetails.Web
open eShop.Domain.ConferenceManagement.CreateConference.Web
open eShop.Domain.ConferenceManagement.EditConference.Web
open eShop.Domain.ConferenceManagement.LocateConference.Web
open eShop.Domain.ConferencePublic.DisplayConference.Web

[<AutoOpen>]
module Middleware =

    open Microsoft.AspNetCore.Mvc.Razor

    type IServiceCollection with
        member this.AddRazorEngine =
            this.Configure<RazorViewEngineOptions>(
                fun (options : RazorViewEngineOptions) ->
                    options.ViewLocationFormats.Clear()
                    options.ViewLocationFormats.Add("/eShop.Domain.ConferenceManagement/{1}/{0}.cshtml")
                    options.ViewLocationFormats.Add("/eShop.Domain.ConferencePublic/{1}/{0}.cshtml")
                )
                .AddMvc()
            |> ignore
            this.AddAntiforgery()

// ---------------------------------
// Web app
// ---------------------------------

let webApp =
    choose [
        GET >=>
            choose [
                route  "/conferences"            >=> renderHomeView
                route  "/conferences/create"     >=> renderCreateConferenceView
                route  "/conferences/details"    >=> renderConferenceDetailsView
                route  "/conferences/locate"     >=> renderLocateConferenceView
                route  "/conferences/edit"       >=> renderEditConferenceView

                // TODO: change route
                route  "/conferences/display"    >=> renderDisplayConferenceView
            ]
        POST >=>
            choose [
                route  "/conferences/create"     >=> createConference
                route  "/conferences/locate"     >=> locateConference
                route  "/conferences/edit"       >=> updateConference
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