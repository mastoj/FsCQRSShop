namespace FsCQRSShop.Domain
open FsCQRSShop.Contract
open Types
open System
open Events
open Railway

module State =

    type ProductState = {Id:ProductId; Name:string; Price: int}
    let initProduct = {Id = ProductId(Guid.Empty); Name = ""; Price = 0}

    type Dependencies = {readEvents: Guid -> (int*Event list)}
    let evolve evolveOne initState events =
        List.fold (fun result e -> match result with
                                   | Failure f -> Failure f
                                   | Success (v,s) -> match (evolveOne s e) with
                                                      | Success s -> Success (v+1, s) 
                                                      | Failure f -> Failure f) 
                  (Success (0, initState)) events

    let getTypeName o = o.GetType().Name
    let stateTransitionFail event state = Failure (InvalidStateTransition (sprintf "Invalid event %s for state %s" (event |> getTypeName) (state |> getTypeName)))
