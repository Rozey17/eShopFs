module eShop.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open eShop.Domain.Conference.Web
open eShop.Domain.Registration

[<AutoOpen>]
module Middleware =

    open Microsoft.AspNetCore.Mvc.Razor

    type IServiceCollection with
        member this.AddRazorEngine =
            this.Configure<RazorViewEngineOptions>(
                fun (options : RazorViewEngineOptions) ->
                    options.ViewLocationFormats.Clear()
                    options.ViewLocationFormats.Add("/eShop.Domain.Conference.Web/{1}/{0}.cshtml")
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
                route  "/conferences"            >=>  Home.Impl.renderHomeView
                route  "/conferences/create"     >=>  CreateConference.Impl.renderCreateConferenceView
                route  "/conferences/details"    >=>  ConferenceDetails.renderConferenceDetailsView
                route  "/conferences/locate"     >=>  LocateConference.renderLocateConferenceView
                route  "/conferences/edit"       >=>  EditConference.renderEditConferenceView
            ]
        POST >=>
            choose [
                route  "/conferences/create"     >=>  CreateConference.Impl.createConference
                route  "/conferences/locate"     >=>  LocateConference.locateConference
                route  "/conferences/edit"       >=>  EditConference.updateConference
                route  "/conferences/publish"    >=>  PublishConference.publishConference
                route  "/conferences/unpublish"  >=>  UnpublishConference.unpublishConference
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
    Conference.Integrator.initialise()

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