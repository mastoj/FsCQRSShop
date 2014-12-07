// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open FsCQRSShop.Contract
open FsCQRSShop.Infrastructure
open FsCQRSShop.Domain
open FsCQRSShop.Application
open Types
open Builder
open Newtonsoft.Json
open EventStore.DummyEventStore
open Commands
open Events


module Test = 

    let doStuff = 
        let es = create()
        let appendStream = appendToStream es
        let readStream = readFromStream es
//        let application = createApplication readStream appendStream

        let fnr = fødselsnummer "08080812345" |> Option.get
        let id = Guid.NewGuid()
        let personId = PersonId(id)
        let adresse = {Linjer = "hello"}
//        let c1 = PersonCommand(OpprettFraDsf(personId, fnr, "jansson"))
//        let c2 = PersonCommand(RegistrerAdresse(personId, adresse))
//        application c1
//        application c2
        
        let (version, events) = readFromStream es (toStreamId id)

        let serialized = JsonConvert.SerializeObject(events)
        printfn "serialized: %s" serialized

        let deserialized = JsonConvert.DeserializeObject<Event list>(serialized)
        printfn "deserialized: %A" deserialized
        printfn "deserialized: %O" deserialized

        let es = create()
        appendToStream es "test" -1 events
        let (version, readEvents) = readFromStream es "test"
        printf "Read: %A, Version: %d" readEvents version

open Test
[<EntryPoint>]
let main argv = 
    doStuff
    let s = Console.ReadLine()
    0 // return an integer exit code
