namespace FsCQRSShop.Web

module Model =
    open System
    open FsCQRSShop.Contract
    open Commands
    open Types
    open FsCQRSShop.Domain.Railway

    type CreateCustomerDto = {Name: string}
    type MarkCustomerAsPreferredDto = {CustomerId: Guid; Discount: int}
    type CreateProductDto = {Name: string; Price: int}
    type CreateBasketDto = {CustomerId: Guid}
    type AddItemToBasketDto = {BasketId: Guid; ProductId: Guid; Quantity: int}
    type ProceedToCheckoutDto = {BasketId: Guid}
    type CheckoutBasketDto = {BasketId: Guid; ShippingAddress: Address}
    type MakePaymentDto = {BasketId: Guid; Payment: int}
    type StartShippingProcessDto = {OrderId:Guid}
    type CancelOrderDto = {OrderId:Guid}
    type ShipOrderDto = {OrderId:Guid}
    type ApproveOrderDto = {OrderId:Guid}

    let toCommand (dto: obj) = 
        match dto with
        | :? CreateCustomerDto as d -> Success (CustomerCommand(CreateCustomer(CustomerId (Guid.NewGuid()), d.Name)))
        | :? MarkCustomerAsPreferredDto as d -> Success (CustomerCommand(MarkCustomerAsPreferred(CustomerId(d.CustomerId), d.Discount)))
        | :? CreateProductDto as d -> Success (ProductCommand(CreateProduct(ProductId(Guid.NewGuid()), d.Name, d.Price)))
        | :? CreateBasketDto as d -> Success (BasketCommand(CreateBasket(BasketId(Guid.NewGuid()), CustomerId(d.CustomerId))))
        | :? AddItemToBasketDto as d -> Success (BasketCommand(AddItemToBasket(BasketId(d.BasketId), ProductId(d.ProductId), d.Quantity)))
        | :? ProceedToCheckoutDto as d -> Success (BasketCommand(ProceedToCheckout(BasketId(d.BasketId))))
        | :? CheckoutBasketDto as d -> Success (BasketCommand(CheckoutBasket(BasketId(d.BasketId), d.ShippingAddress)))
        | :? MakePaymentDto as d -> Success (BasketCommand(MakePayment(BasketId(d.BasketId), d.Payment)))
        | :? StartShippingProcessDto as d -> Success (OrderCommand(StartShippingProcess(OrderId(d.OrderId))))
        | :? CancelOrderDto as d -> Success (OrderCommand(CancelOrder(OrderId(d.OrderId))))
        | :? ShipOrderDto as d -> Success (OrderCommand(ShipOrder(OrderId(d.OrderId))))
        | :? ApproveOrderDto as d -> Success (OrderCommand(ApproveOrder(OrderId(d.OrderId))))
        | _ -> Failure (UnknownDto (dto.GetType().Name))
