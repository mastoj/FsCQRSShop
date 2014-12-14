namespace FsCQRSShop.Web

module Controllers = 
    open WebStart
    open System
    open System.Web.Http
    open System.Net.Http
    open System.Net
    open Model

    [<RoutePrefix("api")>]
    type HomeController() =
        inherit ApiController()
        [<Route>]
        member this.Get() = 
            this.Request.CreateResponse(HttpStatusCode.OK, "Welcome sir")

    [<RoutePrefix("api/customer")>]
    type AccountController() = 
        inherit ApiController()
        [<Route>]
        member this.Post (dto:CreateCustomerDto) = dto |> app this

        [<Route("{CustomerId}/prefer")>]
        member this.Post (customerId:Guid, dto:MarkCustomerAsPreferredDto) = {dto with CustomerId = customerId} |> app this

    [<RoutePrefix("api/product")>]
    type ProductController() = 
        inherit ApiController()
        [<Route>]
        member this.Post (dto:CreateProductDto) = dto |> app this

    [<RoutePrefix("api/basket")>]
    type BasketController() = 
        inherit ApiController()
        [<Route>]
        member this.Post (dto:CreateBasketDto) = dto |> app this

        [<Route("{BasketId}/additem")>]
        member this.PostAdd (basketId:Guid, dto:AddItemToBasketDto) = {dto with BasketId = basketId} |> app this
        
        [<Route("{BasketId}/checkout")>]
        member this.PostCheckout (basketId:Guid, dto:CheckoutBasketDto) = {dto with BasketId = basketId} |> app this

        [<Route("{BasketId}/pay")>]
        member this.PostPay (basketId:Guid, dto:MakePaymentDto) = {dto with BasketId = basketId} |> app this

        [<Route("{BasketId}/proceedtocheckout")>]
        member this.PostProceed (basketId:Guid, dto:ProceedToCheckoutDto) = {dto with BasketId = basketId} |> app this

    [<RoutePrefix("api/order")>]
    type OrderController() = 
        inherit ApiController()
        [<Route("{OrderId}/approve")>]
        member this.PostApprove (orderId:Guid, dto:ApproveOrderDto) = {dto with OrderId = orderId} |> app this

        [<Route("{OrderId}/cancel")>]
        member this.PostCancel (orderId:Guid, dto:CancelOrderDto) = {dto with OrderId = orderId} |> app this

        [<Route("{OrderId}/Ship")>]
        member this.PostShip (orderId:Guid, dto:ShipOrderDto) = {dto with OrderId = orderId} |> app this

        [<Route("{OrderId}/StartShipping")>]
        member this.PostStartShipping (orderId:Guid, dto:StartShippingProcessDto) = {dto with OrderId = orderId} |> app this

