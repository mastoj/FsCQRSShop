module FsCQRSShop.Tests.``When making customer preferred``

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
let ``the customer should get the discount``() =
    let id = Guid.NewGuid()
    Given ([CustomerCreated(CustomerId id, "tomas jansson")], None)
    |> When (Command.CustomerCommand(MarkCustomerAsPreferred(CustomerId id, 80)))
    |> Expect [CustomerMarkedAsPreferred(CustomerId id, 80)]

[<Fact>]
let ``it should fail if customer doesn't exist``() =
    let id = Guid.NewGuid()
    Given ([], None)
    |> When (Command.CustomerCommand(MarkCustomerAsPreferred(CustomerId id, 80)))
    |> ExpectThrows<InvalidStateException>
