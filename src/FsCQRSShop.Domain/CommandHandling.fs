module FsCQRSShop.Domain.CommandHandling
open System
open FsCQRSShop.Contract
open Commands
open Events
open Customer
open Basket
open Order
open Product
open State

open FsCQRSShop.Domain.EventHandling

let handle deps c =
    match c with
    | Command.BasketCommand(bc) -> handleBasket deps bc
    | Command.CustomerCommand(cc) -> handleCustomer deps cc
    | Command.OrderCommand(oc) -> handleOrder deps oc
    | Command.ProductCommand(pc) -> handleProduct deps pc
