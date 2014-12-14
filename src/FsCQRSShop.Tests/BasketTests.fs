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
open FsCQRSShop.Domain.Railway

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
        |> ExpectFail (InvalidState "Customer")

    [<Fact>]
    let ``it should fail if basket id is not unique``() = 
        let customerGuid = Guid.NewGuid()
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId (basketGuid)
        let customerId = CustomerId (customerGuid)
        Given ([(customerGuid, [CustomerCreated(customerId, "john doe")]); 
                (basketGuid, [BasketCreated(basketId, customerId, 0)])], None)
        |> When (Command.BasketCommand(CreateBasket(basketId, customerId)))
        |> ExpectFail (InvalidState "Basket")

    [<Fact>]
    let ``the customer should get its discount``() = 
        let customerGuid = Guid.NewGuid()
        let basketId = BasketId (Guid.NewGuid())
        let customerId = CustomerId (customerGuid)
        Given ([(customerGuid, [CustomerCreated(customerId, "john doe"); CustomerMarkedAsPreferred(customerId, 10)])], None)
        |> When (Command.BasketCommand(CreateBasket(basketId, customerId)))
        |> Expect [BasketCreated(basketId, customerId, 10)]

module ``When adding an item to the basket`` = 
    [<Fact>]
    let ``a regular customer shouldn't get any discount``() =
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId(basketGuid)
        let productGuid = Guid.NewGuid()
        let productId = ProductId(productGuid)
        let productName = "ball"
        let productPrice = 100
        let quantity = 10
        let orderLine = {ProductId = productId; ProductName = productName; OriginalPrice = productPrice; DiscountedPrice = productPrice; Quantity = quantity}
        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), 0)]);
               (productGuid, [ProductCreated(productId, productName, productPrice)])], None)
        |> When (Command.BasketCommand(AddItemToBasket(basketId, productId, quantity)))
        |> Expect [ItemAdded(basketId, orderLine)]

    [<Fact>]
    let ``a preferred customer should get its discount``() =
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId(basketGuid)
        let productGuid = Guid.NewGuid()
        let productId = ProductId(productGuid)
        let productName = "ball"
        let productPrice = 100
        let quantity = 10
        let discountPercentage = 40
        let orderLine = {ProductId = productId; ProductName = productName; 
                         OriginalPrice = productPrice; DiscountedPrice = productPrice - (productPrice * discountPercentage)/100; 
                         Quantity = quantity}
        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), discountPercentage)]);
               (productGuid, [ProductCreated(productId, productName, productPrice)])], None)
        |> When (Command.BasketCommand(AddItemToBasket(basketId, productId, quantity)))
        |> Expect [ItemAdded(basketId, orderLine)]