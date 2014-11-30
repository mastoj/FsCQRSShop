module FsCQRSShop.Contract.Events

open Types

type Event = 
    | CustomerCreated of Id:CustomerId * Name:string
    | CustomerMarkedAsPreferred of Id:CustomerId * Discount:int
    | ProductCreated of Id:ProductId * Name:string * Price:int
    | BasketCreated of Id:BasketId * CustomerId:CustomerId * Discount:int
    | ItemAdded of Id:BasketId * OrderLine:OrderLine
    | CustomerIsCheckoutOutBasket of Id:BasketId
    | BasketCheckedOut of Id:BasketId * ShippingAddress:Address
    | OrderCreated of Id:OrderId * BasketId:BasketId * OrderLines:OrderLine list
    | ShippingProcessStarted of Id:OrderId
    | OrderCancelled of Id:OrderId
    | OrderShipped of Id:OrderId
    | NeedsApproval of Id:OrderId
    | OrderApproved of Id:OrderId

