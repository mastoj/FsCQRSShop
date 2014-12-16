namespace FsCQRSShop.Domain
open System
open FsCQRSShop.Contract
open Commands
open Events
open Customer
open Basket
open Order
open Product
open Helpers

module CommandHandling =
    let handle deps = function
    | Command.CustomerCommand(cc) -> handleCustomer deps cc
    | Command.BasketCommand(bc) -> handleBasket deps bc
    | Command.OrderCommand(oc) -> handleOrder deps oc
    | Command.ProductCommand(pc) -> handleProduct deps pc
