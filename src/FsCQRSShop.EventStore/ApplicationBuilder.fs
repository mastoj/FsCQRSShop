module FsCQRSShop.Infrastructure.ApplicationBuilder
open Railroad

let toStreamId id = sprintf "Data-%O" id

let bind switchFunction = 
    fun input -> match input with
                    | Success s -> switchFunction s
                    | Fail s -> Fail s

let buildApplication save handler = handler >> (bind save)//    (fun c -> handler c |> bind save)