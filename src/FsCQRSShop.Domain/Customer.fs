module FsCQRSShop.Domain.Customer

open System
open FsCQRSShop.Contract
open Types
open Commands
open Events
open State

exception InvalidStateException

let handleCustomer state pc =
    match pc with
    | CreateCustomer(id, name) -> [CustomerCreated(id, name)]
    | MarkCustomerAsPreferred(id, discount) -> [CustomerMarkedAsPreferred(id, discount)]

let evolveCustomer state event =
    match event with
    | CustomerCreated(id, name) -> State.Customer({Id = id; Name = name; Discount = 0} )
    | CustomerMarkedAsPreferred(id, discount) -> Customer({state with Discount = discount})
