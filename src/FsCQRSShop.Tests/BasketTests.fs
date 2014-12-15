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

module ``When checking out a basket`` = 
    [<Fact>]
    let ``the address must be valid (not empty)``() = 
        let address = "  "
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId basketGuid
        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), 0)])], None)
        |> When (Command.BasketCommand(CheckoutBasket(basketId, {Street = address})))
        |> ExpectFail (ValidationError "Invalid address")

    [<Fact>]
    let ``it should be checked out for valid address``() = 
        let address = "   some street   "
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId basketGuid
        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), 0)])], None)
        |> When (Command.BasketCommand(CheckoutBasket(basketId, {Street = address})))
        |> Expect [BasketCheckedOut(basketId, {Street = address.Trim()})]

module ``When proceeding to checkout`` =
    [<Fact>]
    let ``the basket should be marked as checking out``() =
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId basketGuid
        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), 0)])], None)
        |> When (Command.BasketCommand(ProceedToCheckout(basketId)))
        |> Expect [CustomerIsCheckoutOutBasket(basketId)]

module ``When making a payment`` = 
    [<Fact>]
    let ``the payment should succeed if the paid amount is the same as the expected``() =
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId basketGuid
        let orderGuid = Guid.NewGuid()
        let orderId = OrderId orderGuid
        let orderLine = {ProductId = ProductId(Guid.NewGuid()); ProductName = "Ball"; OriginalPrice = 100; DiscountedPrice = 100; Quantity = 10}

        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), 0); ItemAdded(basketId, orderLine)])], 
              Some {defaultDependencies with guidGenerator = (fun() -> orderGuid)})
        |> When (Command.BasketCommand(MakePayment(basketId, 1000)))
        |> Expect [OrderCreated(orderId, basketId, [orderLine]); OrderApproved(orderId)]

    [<Fact>]
    let ``the payment should not succeed if the paid amount is not the same as the expected``() =
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId basketGuid
        let orderGuid = Guid.NewGuid()
        let orderId = OrderId orderGuid
        let orderLine = {ProductId = ProductId(Guid.NewGuid()); ProductName = "Ball"; OriginalPrice = 100; DiscountedPrice = 100; Quantity = 10}

        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), 0); ItemAdded(basketId, orderLine)])], 
              Some {defaultDependencies with guidGenerator = (fun() -> orderGuid)})
        |> When (Command.BasketCommand(MakePayment(basketId, 1001)))
        |> ExpectFail InvalidPaymentAmount

    [<Fact>]
    let ``of an order larger than 100000 it needs an approval``() =
        let basketGuid = Guid.NewGuid()
        let basketId = BasketId basketGuid
        let orderGuid = Guid.NewGuid()
        let orderId = OrderId orderGuid
        let orderLine = {ProductId = ProductId(Guid.NewGuid()); ProductName = "Ball"; OriginalPrice = 100; DiscountedPrice = 100001; Quantity = 1}

        Given([(basketGuid, [BasketCreated(basketId, CustomerId(Guid.NewGuid()), 0); ItemAdded(basketId, orderLine)])], 
              Some {defaultDependencies with guidGenerator = (fun() -> orderGuid)})
        |> When (Command.BasketCommand(MakePayment(basketId, 100001)))
        |> Expect [OrderCreated(orderId, basketId, [orderLine]); NeedsApproval(orderId)]
        