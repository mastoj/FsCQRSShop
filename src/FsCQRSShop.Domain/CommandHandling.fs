module FsCQRSShop.Domain.CommandHandling

open FsCQRSShop.Contract
open Commands
open Customer
open Basket
open Order
open Product

//type Dependencies = {}

let handle c state =
    match c with
    | Command.BasketCommand(bc) -> handleBasket state bc
    | Command.CustomerCommand(cc) -> handleCustomer state cc
    | Command.OrderCommand(oc) -> handleOrder state oc
    | Command.ProductCommand(pc) -> handleProduct state pc
