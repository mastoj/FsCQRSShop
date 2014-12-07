module FsCQRSShop.Tests.Specification
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

open FsCQRSShop.Infrastructure.EventStore.DummyEventStore
open FsCQRSShop.Infrastructure.ApplicationBuilder
open FsCQRSShop.Infrastructure.Railroad

type TestSpec = {PreCondition: ((Guid*Event list) list * Dependencies option); Action: Command; PostCondition: Event list}

let createTestApplication dependencies events = 
    let es = create()
    let toStreamId (id:Guid) = sprintf "%O" id
    let readStream id = readFromStream es (toStreamId id)
    events |> List.map (fun (id, evts) -> appendToStream es (toStreamId id) -1 evts) |> ignore
    let deps = {readEvents = readStream}

    let save res = Success res
    let handler = handle deps
    buildApplication save handler

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

let ExpectFail (events, dependencies, command) =
    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Throws: %A" typeof<'Ex>

    command
    |> (createTestApplication dependencies events)
    |> (fun x -> match x with
                    | Success _ -> true
                    | Fail _ -> false)
    |> should equal false

//    (fun () -> command |> (createTestApplication dependencies events) |> ignore) 
//    |> should throw typeof<'Ex>