namespace FsCQRSShop.Domain
open FsCQRSShop.Contract
open Types
open System
open Events
open Railway

module Helpers =
    let evolve evolveOne initState =
        List.fold (fun result e -> 
              result >>= fun (v,s) ->
                  evolveOne s e >>= fun s -> Success (v+1, s))
            (Success (-1, initState)) 

    let getTypeName o = o.GetType().Name
    let stateTransitionFail event state = Failure (InvalidStateTransition (sprintf "Invalid event %s for state %s" (event |> getTypeName) (state |> getTypeName)))
