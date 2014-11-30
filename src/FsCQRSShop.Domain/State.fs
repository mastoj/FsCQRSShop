module State

open FsCQRSShop.Contract
open Types
open System

type CustomerState = {Id:CustomerId; Name:string; Discount:int} 
let initCustomer = {Id = CustomerId(Guid.Empty); Name = ""; Discount = 0}

type ProductState = {Id:ProductId; Name:string; Price: int}
let initProduct = {Id = ProductId(Guid.Empty); Name = ""; Price = 0}

type State =
    | Init
    | Customer of CustomerState
    | Product of ProductState
