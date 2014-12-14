namespace FsCQRSShop.Domain
module Basket =
    open System
    open FsCQRSShop.Contract
    open Commands
    open Events
    open Types
    open Railway
    open Customer
    open Helpers
    open Product

    type BasketInfo = {Id: BasketId; Discount: int}
    type Basket = 
    | Init
    | Created of BasketInfo

    let evolveOneBasket state event = 
        match event with
        | BasketCreated(basketId, customerId, discount) -> Success (Created {Id = basketId; Discount = discount})
    let evolveBasket = evolve evolveOneBasket

    let handleBasket deps pc = 
        let getState id = evolveBasket Init ((deps.readEvents id) |> (fun (_, e) -> e))

        let createBasket id customerId discountResult (version,state) = 
            match state, discountResult with
            | Init, (Success discount) -> Success (id, version, [BasketCreated(BasketId id, customerId, discount)])
            | _, Failure f -> Failure f
            | _, _ -> Failure (InvalidState "Basket")

        let createOrderLine (product:ProductInfo) quantity basket = 
             {ProductId = product.Id; ProductName = product.Name; OriginalPrice = product.Price; 
              DiscountedPrice = product.Price - (product.Price * basket.Discount / 100); Quantity = quantity}

        let addItem basketId productId quantity (version,state) = 
            match state with
            | Init -> Failure (InvalidState "Basket")
            | Created bi -> 
                getProduct deps productId
                >>= (fun (_, product) -> match product with
                                         | Product.Created p -> 
                                            let orderLine = createOrderLine p quantity bi
                                            Success (basketId, version, [ItemAdded(BasketId basketId, orderLine)])
                                         | _ -> Failure (InvalidState "Product"))

        match pc with
        | CreateBasket(BasketId id, CustomerId customerId) ->
            let discountResult = getCustomerState deps customerId
                                 >>= (fun (v, c) -> Success c)
                                 >>= getDiscount
            getState id
            >>= createBasket id (CustomerId customerId) discountResult
        | AddItemToBasket(BasketId basketId, ProductId productId, quantity) ->
            getState basketId
            >>= addItem basketId productId quantity

        | _ -> Failure (NotSupportedCommand (pc.GetType().Name))
