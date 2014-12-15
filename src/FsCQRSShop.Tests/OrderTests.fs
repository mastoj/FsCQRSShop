namespace FsCQRSShop.Tests.OrderTests
open System
open Xunit
open FsCQRSShop.Tests.Specification
open FsCQRSShop.Contract.Events
open FsCQRSShop.Contract.Types
open FsCQRSShop.Contract.Commands
open FsCQRSShop.Domain.Railway

[<AutoOpen>]
module OrderTestsHelper =
    let createAnOrderLine = {ProductId = ProductId (Guid.NewGuid()); ProductName = "Name"; OriginalPrice = 100; DiscountedPrice = 100; Quantity = 0}
    let orderLines = [(createAnOrderLine)]

    let givenOrderIsCreatedWith id extraEvents =
        Given ([(id, (OrderCreated(OrderId id, BasketId (Guid.NewGuid()), orderLines)::extraEvents))], None)

module ``When you start the shipping process`` =
    [<Fact>]
    let ``the shipping process should be started``() =
        let id = Guid.NewGuid()
        givenOrderIsCreatedWith id []
        |> When (Command.OrderCommand(StartShippingProcess(OrderId id)))
        |> Expect [ShippingProcessStarted(OrderId id)]

    [<Fact>]
    let ``of a cancelled order the start should fail``() =
        let id = Guid.NewGuid()
        givenOrderIsCreatedWith id [OrderCancelled(OrderId id)]
        |> When (Command.OrderCommand(StartShippingProcess(OrderId id)))
        |> ExpectFail (InvalidState "Order")

module ``When you cancel an order`` =
    [<Fact>]
    let ``that hasn't been shipped the order should be cancelled``() =
        let id = Guid.NewGuid()
        givenOrderIsCreatedWith id []
        |> When (Command.OrderCommand(CancelOrder(OrderId id)))
        |> Expect [OrderCancelled(OrderId id)]

    [<Fact>]
    let ``that is about to ship the cancellation should fail``() =
        let id = Guid.NewGuid()
        givenOrderIsCreatedWith id [ShippingProcessStarted(OrderId id)]
        |> When (Command.OrderCommand(CancelOrder(OrderId id)))
        |> ExpectFail (InvalidState "Order")

module ``When you shipping an order`` =
    [<Fact>]
    let ``that has started its shipping process it should ship``() =
        let id = Guid.NewGuid()
        givenOrderIsCreatedWith id [ShippingProcessStarted(OrderId id)]
        |> When (Command.OrderCommand(ShipOrder(OrderId id)))
        |> Expect [OrderShipped(OrderId id)]

    [<Fact>]
    let ``that hasn't started its shipping process it should fail``() =
        let id = Guid.NewGuid()
        givenOrderIsCreatedWith id []
        |> When (Command.OrderCommand(ShipOrder(OrderId id)))
        |> ExpectFail (InvalidState "Order")
        
module ``When approving an order`` =
    [<Fact>]
    let ``the order should get approved``() =
        let id = Guid.NewGuid()
        givenOrderIsCreatedWith id []
        |> When (Command.OrderCommand(ApproveOrder(OrderId id)))
        |> Expect [OrderApproved(OrderId id)]