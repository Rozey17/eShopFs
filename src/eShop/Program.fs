module eShop.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open eShop.Domain.Conference.Web
open eShop.Domain.Registration.Conference.Integrator
open eShop.Domain.Registration.Web

[<AutoOpen>]
module Middleware =

    open Microsoft.AspNetCore.Mvc.Razor

    type IServiceCollection with
        member this.AddRazorEngine =
            this.Configure<RazorViewEngineOptions>(
                fun (options : RazorViewEngineOptions) ->
                    options.ViewLocationFormats.Clear()
                    options.ViewLocationFormats.Add("/eShop.Domain.Conference.Web/{1}/{0}.cshtml")
                    options.ViewLocationFormats.Add("/eShop.Domain.Registration.Web/{1}/{0}.cshtml")
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
                route  "/conferences"                >=>  ConferencesHome.Impl.renderHomeView
                route  "/conferences/create"         >=>  CreateConference.Impl.renderCreateConferenceView
                route  "/conferences/details"        >=>  ConferenceDetails.Impl.renderConferenceDetailsView
                route  "/conferences/locate"         >=>  LocateConference.Impl.renderLocateConferenceView
                route  "/conferences/edit"           >=>  EditConference.Impl.renderEditConferenceView
                route  "/conferences/seats"          >=>  Seats.Impl.renderSeatsView
                route  "/conferences/create-seat"    >=>  CreateSeat.Impl.renderCreateSeatView
                route  "/conferences/edit-seat"      >=>  EditSeat.Impl.renderEditSeatView

                route  "/registration"               >=>  RegistrationHome.Impl.renderHomeView
                route  "/registration/conference"    >=>  DisplayConference.Impl.renderDisplayConferenceView
            ]
        POST >=>
            choose [
                route  "/conferences/create"         >=>  CreateConference.Impl.createConference
                route  "/conferences/locate"         >=>  LocateConference.Impl.locateConference
                route  "/conferences/edit"           >=>  EditConference.Impl.updateConference
                route  "/conferences/publish"        >=>  PublishConference.Impl.publishConference
                route  "/conferences/unpublish"      >=>  UnpublishConference.Impl.unpublishConference
                route  "/conferences/create-seat"    >=>  CreateSeat.Impl.createSeat
                route  "/conferences/update-seat"    >=>  EditSeat.Impl.updateSeat
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
    ConferenceIntegrator.initialise()

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