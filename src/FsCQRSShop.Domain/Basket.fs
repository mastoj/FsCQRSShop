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
    | Created of BasketInfo * OrderLine list

    let evolveOneBasket state = function
        | BasketCreated(basketId, customerId, discount) -> Success (Created ({Id = basketId; Discount = discount},[]))
        | ItemAdded(basketId, orderLine) -> match state with
                                            | Created (info, lines) -> Success (Created(info, orderLine::lines))
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
            | Created (bi,_) -> 
                getProduct deps productId
                >>= (fun (_, product) -> match product with
                                         | Product.Created p -> 
                                            let orderLine = createOrderLine p quantity bi
                                            Success (basketId, version, [ItemAdded(BasketId basketId, orderLine)])
                                         | _ -> Failure (InvalidState "Product"))

        let checkout id address (version, basket) =
            match basket with
            | Init -> Failure (InvalidState "Basket")
            | Created (b,_) -> Success (id, version, [BasketCheckedOut(b.Id, address)])
        
        let rec calculateOrderTotal lines total =
            match lines with
            | [] -> total
            | x::xs -> calculateOrderTotal xs (total + x.DiscountedPrice * x.Quantity)
    
        let makePayment id payment (version, basket) =
            match basket with
            | Created(b,lines) -> 
                let orderGuid = deps.guidGenerator()
                let orderId = OrderId orderGuid
                let orderTotal = calculateOrderTotal lines 0
                match orderTotal - payment with
                | 0 -> 
                    let events = if orderTotal > 100000 then [NeedsApproval(orderId)] else [OrderApproved(orderId)]
                    Success (orderGuid, -1, (OrderCreated(orderId, b.Id, lines)::events))
                | _ -> Failure InvalidPaymentAmount

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
        | CheckoutBasket(BasketId id, address) ->
            getState id >>= checkout id address
        | ProceedToCheckout(BasketId id) ->
            getState id >>= (fun (v,s) -> Success (id, v, [CustomerIsCheckoutOutBasket(BasketId id)]))
        | MakePayment(BasketId id, payment) ->
            getState id >>= makePayment id payment
        | _ -> Failure (NotSupportedCommand (pc.GetType().Name))
