module FsCQRSShop.Domain.Product

open FsCQRSShop.Contract
open Commands
open Events
open Types

open State
open Exceptions

let evolveOneProduct state event = 
    match event with
    | ProductCreated(id, name, price) -> {Id = id; Name = name; Price = price}

let evolveProduct = evolve evolveOneProduct

let handleProduct deps pc = 
    let getState (ProductId id) = evolveProduct initProduct ((deps.readEvents id) |> (fun (_, e) -> e))
    match pc with
    | CreateProduct(id, name, price) -> 
        let state = getState id
        if state <> initProduct then raise InvalidStateException
        [ProductCreated(id, name, price)]