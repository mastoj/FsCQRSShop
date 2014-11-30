module FsCQRSShop.Domain.Product

open FsCQRSShop.Contract
open Commands
open Events
open Types

open State
open Exceptions

let handleProduct state pc = 
    let productState = match state with
                        | Product ps -> ps
                        | _ -> raise InvalidStateException
    match pc with
    | CreateProduct(id, name, price) -> 
        if productState <> initProduct then raise InvalidStateException
        [ProductCreated(id, name, price)]

let evolveProduct state event =
    match event with
    | ProductCreated(id, name, price) -> State.Product({Id = id; Name = name; Price = price})