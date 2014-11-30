module FsCQRSShop.Tests.``When creating a customer``
open System
open Xunit
open FsUnit.Xunit

open FsCQRSShop.Contract
open Events
open Commands
open Types

open FsCQRSShop.Domain
open EventHandling
open CommandHandling
open State

type Dependencies = {GetPerson: (string -> Fødselsnummer -> DsfPerson) option}
type TestSpec = {PreCondition: (Event list * Dependencies option); Action: Command; PostCondition: Event list}

let Given (events: Event list, dependencies: Dependencies option) = events, dependencies
let When (command: Command) (events: Event list, dependencies: Dependencies option) = events, dependencies, command
let Expect (expectedEvents:Event list) (events:Event list, dependencies: Dependencies option, command: Command) = 
    evolve command events |> handle command |> should equal expectedEvents

[<Fact>]
let ``the customer should be created``() =
    let id = Guid.NewGuid()
    Given ([], None)
    |> When (Command.CustomerCommand(CreateCustomer(CustomerId id, "tomas jansson")))
    |> Expect [CustomerCreated(CustomerId id, "tomas jansson")]
