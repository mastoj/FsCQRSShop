module FsCQRSShop.Infrastructure.EventStore
open FsCQRSShop.Contract
open Events

exception VersionError

[<AutoOpen>]
module AsyncExtensions =
    open System
    open System.Threading
    open System.Threading.Tasks
    type Microsoft.FSharp.Control.Async with
        static member Raise(ex) = Async.FromContinuations(fun (_,econt,_) -> econt ex)

        static member AwaitTask (t: Task) =
            let tcs = new TaskCompletionSource<unit>(TaskContinuationOptions.None)
            t.ContinueWith((fun _ -> 
                if t.IsFaulted then tcs.SetException t.Exception
                elif t.IsCanceled then tcs.SetCanceled()
                else tcs.SetResult(())), TaskContinuationOptions.ExecuteSynchronously) |> ignore
            async {
                try
                    do! Async.AwaitTask tcs.Task
                with
                | :? AggregateException as ex -> 
                    do! Async.Raise (ex.Flatten().InnerExceptions |> Seq.head) }

    open EventStore.ClientAPI

    type IEventStoreConnection with
        member this.AsyncConnect() = Async.AwaitTask(this.ConnectAsync())
        member this.AsyncReadStreamEventsForward stream start count resolveLinkTos =
            Async.AwaitTask(this.ReadStreamEventsForwardAsync(stream, start, count, resolveLinkTos))
        member this.AsyncAppendToStream stream expectedVersion events =
            Async.AwaitTask(this.AppendToStreamAsync(stream, expectedVersion, events))
        member this.AsyncSubscribeToAll(resolveLinkTos, eventAppeared, userCredentials) =
            Async.AwaitTask(this.SubscribeToAllAsync(resolveLinkTos, eventAppeared, userCredentials = userCredentials))

module DummyEventStore = 
    type EventStream = {mutable Events: (Event * int) list} with
        member this.addEvents events = (this.Events <- events) |> ignore
        static member Version stream =
            stream.Events |> List.last |> snd

    type EventStore = {mutable Streams : Map<string, EventStream> }

    let create() = {Streams = Map.empty }

    let getStreamWithVersion store streamId expectedVersion = 
        match store.Streams.TryFind streamId with
            | Some s when EventStream.Version s = expectedVersion -> s
            | None when expectedVersion = -1 -> 
                let s = {Events = List.empty}
                store.Streams <- store.Streams.Add(streamId, s)
                s
            | _ -> raise VersionError

    let appendToStream store streamId expectedVersion newEvents = 
        let stream = getStreamWithVersion store streamId expectedVersion
        let eventsWithVersion = newEvents |> List.mapi (fun index event -> (event, expectedVersion + index + 1)) 
        let x = (List.append stream.Events eventsWithVersion)
        stream.addEvents x

    let readFromStream store streamId =
        match store.Streams.TryFind streamId with
        | Some s -> s.Events |> List.fold (fun (maxV, es) (e, v) -> (v, es @ [e])) (0, [])
        | None -> (-1, [])

module EventStore = 
    open System
    open System.Net
    open EventStore.ClientAPI

    let connect() = 
        let endpoint = new IPEndPoint(IPAddress.Loopback, 2113)
        let connection = EventStoreConnection.Create(endpoint)
        connection.AsyncConnect() |> Async.RunSynchronously
        connection

