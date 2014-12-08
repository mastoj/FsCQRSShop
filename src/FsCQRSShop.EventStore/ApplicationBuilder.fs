module FsCQRSShop.Infrastructure.ApplicationBuilder
open Railroad

let toStreamId id = sprintf "Data-%O" id

let bind switchFunction = 
    fun input -> match input with
                    | Success s -> switchFunction s
                    | Fail s -> Fail s


let (>>=) input switchFunction = bind switchFunction input

let buildApplication save handler c = 
    (handler c) >>= save //    (fun c -> handler c |> bind save)