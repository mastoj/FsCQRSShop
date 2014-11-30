module FsCQRSShop.Tests.``When creating a customer``
open System
open Xunit
open FsUnit.Xunit

open FsCQRSShop.Contract
open Events
open Commands
open Types

open FsCQRSShop.Domain
open Exceptions

open Specification

[<Fact>]
let ``the customer should be created``() =
    let id = Guid.NewGuid()
    Given ([], None)
    |> When (Command.CustomerCommand(CreateCustomer(CustomerId id, "tomas jansson")))
    |> Expect [CustomerCreated(CustomerId id, "tomas jansson")]

[<Fact>]
let ``it should fail if customer exists``() =
    let id = Guid.NewGuid()
    Given ([Event.CustomerCreated(CustomerId id, "john doe")], None)
    |> When (Command.CustomerCommand(CreateCustomer(CustomerId id, "tomas jansson")))
    |> ExpectThrows<InvalidStateException>
