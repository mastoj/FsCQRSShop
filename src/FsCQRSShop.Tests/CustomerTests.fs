namespace FsCQRSShop.Tests.CustomerTests
open System
open Xunit
open FsUnit.Xunit

open FsCQRSShop.Contract
open Events
open Commands
open Types

open FsCQRSShop.Domain

open FsCQRSShop.Tests.Specification
open FsCQRSShop.Domain.Railway

module ``When creating a customer`` =

    [<Fact>]
    let ``the customer should be created``() =
        let id = Guid.NewGuid()
        Given ([], None)
        |> When (Command.CustomerCommand(CreateCustomer(CustomerId id, "tomas jansson")))
        |> Expect [CustomerCreated(CustomerId id, "tomas jansson")]

    [<Fact>]
    let ``it should fail if customer exists``() =
        let id = Guid.NewGuid()
        Given ([(id, [Event.CustomerCreated(CustomerId id, "john doe")])], None)
        |> When (Command.CustomerCommand(CreateCustomer(CustomerId id, "tomas jansson")))
        |> ExpectFail (InvalidState "Customer")

module ``When making customer preferred`` =

    [<Fact>]
    let ``the customer should get the discount``() =
        let id = Guid.NewGuid()
        Given ([(id, [CustomerCreated(CustomerId id, "tomas jansson")])], None)
        |> When (Command.CustomerCommand(MarkCustomerAsPreferred(CustomerId id, 80)))
        |> Expect [CustomerMarkedAsPreferred(CustomerId id, 80)]

    [<Fact>]
    let ``it should fail if customer doesn't exist``() =
        let id = Guid.NewGuid()
        Given ([], None)
        |> When (Command.CustomerCommand(MarkCustomerAsPreferred(CustomerId id, 80)))
        |> ExpectFail (InvalidState "Customer")