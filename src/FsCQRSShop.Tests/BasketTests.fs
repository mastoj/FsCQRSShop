namespace FsCQRSShop.Tests.BasketTests
open System
open Xunit
open FsUnit.Xunit

open FsCQRSShop.Contract
open Events
open Commands
open Types

open FsCQRSShop.Domain

open FsCQRSShop.Tests.Specification

module ``When creating a basket`` = 
    [<Fact>]
    let ``it should succeed if the customer exists``() = 
        let customerGuid = Guid.NewGuid()
        let basketId = BasketId (Guid.NewGuid())
        let customerId = CustomerId (customerGuid)
        Given ([(customerGuid, [CustomerCreated(customerId, "john doe")])], None)
        |> When (Command.BasketCommand(CreateBasket(basketId, customerId)))
        |> Expect [BasketCreated(basketId, customerId, 0)]

    [<Fact>]
    let ``it should fail if customer doesn't exist``() = 
        let basketId = BasketId (Guid.NewGuid())
        let customerId = CustomerId (Guid.NewGuid())
        Given ([], None)
        |> When (Command.BasketCommand(CreateBasket(basketId, customerId)))
        |> ExpectFail

    [<Fact>]
    let `` the customer should get its discount``() = 
        let customerGuid = Guid.NewGuid()
        let basketId = BasketId (Guid.NewGuid())
        let customerId = CustomerId (customerGuid)
        Given ([(customerGuid, [CustomerCreated(customerId, "john doe"); CustomerMarkedAsPreferred(customerId, 10)])], None)
        |> When (Command.BasketCommand(CreateBasket(basketId, customerId)))
        |> Expect [BasketCreated(basketId, customerId, 10)]

