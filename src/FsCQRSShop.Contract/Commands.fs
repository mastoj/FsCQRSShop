module FsCQRSShop.Contract.Commands
open Types

type Command = 
    | PersonCommand of  PersonCommand
    | CustomerCommand of CustomerCommand
    | ProductCommand of ProductCommand
    | BasketCommand of BasketCommand
    | OrderCommand of OrderCommand
and PersonCommand =
    | OpprettFraDsf of PersonId:PersonId * Fødselsnummer:Fødselsnummer * Etternavn:string
    | RegistrerAdresse of PersonId:PersonId * Adresse:Adresse
and CustomerCommand = 
    | CreateCustomer of CustomerId:CustomerId * Name:string
    | MarkCustomerAsPreferred of CustomerId:CustomerId * Discount:int
and ProductCommand =
    | CreateProduct of ProductId:ProductId * Name:string * Price:int
and BasketCommand =
    | CreateBasket of BasketId:BasketId * CustomerId:CustomerId
    | AddItemToBasket of BasketId:BasketId * ProductId:ProductId * Quantity:int
    | ProceedToCheckout of BasketId:BasketId
    | CheckoutBasket of BasketId:BasketId * ShippingAddress:Address
    | MakePayment of BasketId:BasketId * Payment:int
and OrderCommand = 
    | StartShippingProcess of OrderId:OrderId
    | CancelOrder of OrderId:OrderId
    | ShipOrder of OrderId:OrderId
    | ApproveOrder of OrderId:OrderId
