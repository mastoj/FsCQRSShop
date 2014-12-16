namespace FsCQRSShop.Domain
open System
open Railway

module Order =
    open FsCQRSShop.Contract.Types
    open FsCQRSShop.Contract.Events
    open FsCQRSShop.Contract.Commands
    open Helpers

    type Order = 
    | Init
    | Created of OrderId
    | Cancelled of OrderId
    | ShippingStarted of OrderId

    let evolveOneOrder state = function
        | OrderCreated (id, basketId, lines) -> Success (Created(id))
        | OrderCancelled (id) -> Success (Cancelled(id))
        | ShippingProcessStarted (id) -> Success (ShippingStarted(id))

    let evolveOrder = evolve evolveOneOrder
    let getOrder deps id = evolveOrder Init ((deps.readEvents id) |> (fun (_, e) -> e))

    let handleOrder deps pc = 
        let handleCommand id fn =
            getOrder deps id >>= fn

        match pc with
        | StartShippingProcess (OrderId id) -> 
            handleCommand id <| fun (v, s) -> 
                match s with
                | Cancelled _ -> Failure (InvalidState "Order")
                | _ -> Success (id, v, [ShippingProcessStarted(OrderId id)])
        | CancelOrder (OrderId id) -> 
            handleCommand id <| fun (v, s) -> 
                match s with
                | ShippingStarted _ -> Failure (InvalidState "Order")
                | _ -> Success (id, v, [OrderCancelled(OrderId id)])
        | ShipOrder (OrderId id) ->
            handleCommand id <| fun (v,s) -> 
                match s with
                | ShippingStarted _ -> Success (id, v, [OrderShipped(OrderId id)])
                | _ -> Failure (InvalidState "Order")
        | ApproveOrder (OrderId id) ->
            handleCommand id (fun (v,s) -> Success (id, v, [OrderApproved(OrderId id)]))
        | _ -> Failure (NotSupportedCommand (pc.GetType().Name))