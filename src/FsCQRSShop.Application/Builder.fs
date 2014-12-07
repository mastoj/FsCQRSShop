module FsCQRSShop.Application.Builder

open FsCQRSShop.Contract
open FsCQRSShop.Domain
open Types
open Commands
open CommandHandling
open EventHandling

let toStreamId id = sprintf "Data-%O" id

//let createApplication readStream appendStream = 
//    let aggId command = 
//        match command with
//        | CustomerCommand(CreateCustomer(CustomerId id, _)) -> id
//        | CustomerCommand(MarkCustomerAsPreferred(CustomerId id, _)) -> id
//        | ProductCommand(CreateProduct(ProductId id, _, _))
//        | BasketCommand(CreateBasket(BasketId id, _)) -> id
//        | BasketCommand(AddItemToBasket(BasketId id, _, _)) -> id
//        | BasketCommand(ProceedToCheckout(BasketId id)) -> id
//        | BasketCommand(CheckoutBasket(BasketId id, _)) -> id
//        | BasketCommand(MakePayment(BasketId id, _)) -> id
//        | OrderCommand(StartShippingProcess(OrderId id)) -> id
//        | OrderCommand(CancelOrder(OrderId id)) -> id
//        | OrderCommand(ShipOrder(OrderId id)) -> id
//        | OrderCommand(ApproveOrder(OrderId id)) -> id

//    let loadState command id = 
//        let streamId = toStreamId id
//        let (version, events) = readStream streamId
//        let state = evolve command events
//        (version, state)
//        
//    let save id expectedVersion newEvents =
//        let streamId = toStreamId id
//        appendStream streamId expectedVersion newEvents
//
//    let getPerson (x:string) (y:Fødselsnummer) = {Navn = {Fornavn = "Tomas"; Mellomnavn = None; Etternavn = "Jansson"}; Adresse = {Linjer = "Linje 1"}}
//    fun command ->
//        let aggregateId = aggId command
//        let state = loadState command aggregateId 
//        state ||> fun version state -> (version, handle command state) ||> save aggregateId 
