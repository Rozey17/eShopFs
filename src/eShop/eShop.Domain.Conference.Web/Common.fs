namespace eShop.Domain.Conference.Web

open System
open Microsoft.AspNetCore.Http
open Giraffe

[<RequireQualifiedAccess>]
module Common =
    let private notEmpty (str: string) =
        not (String.IsNullOrEmpty str)

    let exnQueryStringValue (ctx: HttpContext) q =
        let v = ctx.TryGetQueryStringValue q

        match v with
        | Some v when (v |> notEmpty) -> v
        | _ -> failwith (sprintf "%s is missing" q)
