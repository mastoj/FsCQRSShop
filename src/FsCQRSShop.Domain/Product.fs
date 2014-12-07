module FsCQRSShop.Domain.Product

open FsCQRSShop.Contract
open Commands
open Events
open Types

open State

open FsCQRSShop.Infrastructure.Railroad

let evolveOneProduct state event = 
    match event with
    | ProductCreated(id, name, price) -> {Id = id; Name = name; Price = price}

let evolveProduct = evolve evolveOneProduct

let handleProduct deps pc = 
    let getState id = evolveProduct initProduct ((deps.readEvents id) |> (fun (_, e) -> e))
    match pc with
    | CreateProduct(ProductId id, name, price) -> 
        let (version, state) = getState id
        if state <> initProduct then Fail "Invalid state"
        else Success (id, version, [ProductCreated(ProductId id, name, price)])