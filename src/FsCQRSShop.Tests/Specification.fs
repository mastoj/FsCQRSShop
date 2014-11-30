module FsCQRSShop.Tests.Specification

open Xunit
open FsUnit.Xunit

open FsCQRSShop.Contract
open Events
open Commands
open Types

open FsCQRSShop.Domain
open EventHandling
open CommandHandling

type Dependencies = {GetPerson: (string -> Fødselsnummer -> DsfPerson) option}
type TestSpec = {PreCondition: (Event list * Dependencies option); Action: Command; PostCondition: Event list}

let Given (events, dependencies) = events, dependencies
let When command (events, dependencies) = events, dependencies, command
let Expect expectedEvents (events, dependencies, command) = 
    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Expects: %A" expectedEvents
    evolve command events |> handle command |> should equal expectedEvents

let ExpectThrows<'Ex> (events, dependencies, command) =
    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Throws: %A" typeof<'Ex>

    (fun () -> evolve command events 
            |> handle command 
            |> ignore) 
    |> should throw typeof<'Ex>