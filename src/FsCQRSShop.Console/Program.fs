// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open FsCQRSShop.Contract
open FsCQRSShop.Infrastructure
open FsCQRSShop.Domain
open Types
open Newtonsoft.Json
open EventStore.EventStore
open Commands
open Events
open Railway


module Test = 
    open CommandHandling
    open DomainBuilder

    let doStuff = 
        let toStreamId (id:Guid) = sprintf "Test-%O" id
        let es = connect()
        let appendStream = appendToStream es
        let readStream id = readFromStream es (toStreamId id)
        let save (id, version, events) = 
            appendStream (toStreamId id) version events |> ignore
            Success events
        let deps = {readEvents = readStream; guidGenerator = Guid.NewGuid}
        let handle = buildDomainEntry save deps

        let id = Guid.NewGuid()
        let command = Command.CustomerCommand(CreateCustomer(CustomerId(id), "Tomas Jansson"))
        let command2 = Command.CustomerCommand(MarkCustomerAsPreferred(CustomerId(id), 80))
        let savedEvents = handle command
        let savedEvents2 = handle command2
        let (version, events) = readStream id

        let serialized = JsonConvert.SerializeObject(events)
        printfn "serialized: %s" serialized

open Test
[<EntryPoint>]
let main argv = 
    doStuff
    let s = Console.ReadLine()
    0 // return an integer exit code
