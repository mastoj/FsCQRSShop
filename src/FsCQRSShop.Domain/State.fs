module State

open FsCQRSShop.Contract
open Types
open System

open Events

type CustomerState = {Id:CustomerId; Name:string; Discount:int} 
let initCustomer = {Id = CustomerId(Guid.Empty); Name = ""; Discount = 0}

type ProductState = {Id:ProductId; Name:string; Price: int}
let initProduct = {Id = ProductId(Guid.Empty); Name = ""; Price = 0}

type BasketState = {Id:BasketId}
let initBasket = {Id = BasketId(Guid.Empty)}

type Dependencies = {readEvents: Guid -> (int*Event list)}
let evolve evolveOne initState events =
    List.fold (fun (v,s) e -> (v + 1, (evolveOne s e))) (0,initState) events

type State =
    | Init
    | Customer of CustomerState
    | Product of ProductState
    | Basket of BasketState
