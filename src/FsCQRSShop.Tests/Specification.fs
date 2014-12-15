module FsCQRSShop.Tests.Specification
open System
open Xunit
open FsUnit.Xunit

open FsCQRSShop.Domain
open CommandHandling
open Helpers

open FsCQRSShop.Domain.DomainBuilder
open FsCQRSShop.Domain.Railway
open FsCQRSShop.Infrastructure.EventStore.DummyEventStore

let defaultDependencies = {readEvents = (fun id -> 0,[]); guidGenerator = Guid.NewGuid}

let createTestApplication dependencies events = 
        let es = create()
        let toStreamId (id:Guid) = sprintf "%O" id
        let readStream id = readFromStream es (toStreamId id)
        events |> List.map (fun (id, evts) -> appendToStream es (toStreamId id) -1 evts) |> ignore
        let deps = match dependencies with
                    | None -> { defaultDependencies with readEvents = readStream}
                    | Some d -> { d with readEvents = readStream }

        let save res = Success res
        buildDomainEntry save deps

let Given (events, dependencies) = events, dependencies
let When command (events, dependencies) = events, dependencies, command

let Expect expectedEvents (events, dependencies, command) = 
    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Expects: %A" expectedEvents
    command 
    |> (createTestApplication dependencies events) 
    |> (fun (Success (id, version, events)) -> events)
    |> should equal expectedEvents

let ExpectFail failure (events, dependencies, command) =
    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Should fail with: %A" failure

    command 
    |> (createTestApplication dependencies events) 
    |> (fun r -> r = Failure failure)
    |> should equal true
