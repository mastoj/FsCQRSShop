module FsCQRSShop.Infrastructure.EventStore
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
        member this.AsyncReadStreamEventsForward stream resolveLinkTos =
            Async.AwaitTask(this.ReadStreamEventsForwardAsync(stream, 0, System.Int32.MaxValue, resolveLinkTos))
        member this.AsyncAppendToStream stream expectedVersion events =
            Async.AwaitTask(this.AppendToStreamAsync(stream, expectedVersion, events))
        member this.AsyncSubscribeToAll(resolveLinkTos, eventAppeared, userCredentials) =
            Async.AwaitTask(this.SubscribeToAllAsync(resolveLinkTos, eventAppeared, userCredentials = userCredentials))

module DummyEventStore = 
    open FsCQRSShop.Contract
    open Events
    type EventStream = {mutable Events: (Event * int) list} with
        member this.addEvents events = (this.Events <- events) |> ignore
        static member Version stream =
            stream.Events |> List.map snd |> List.max

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
    open Newtonsoft.Json
    open EventStore.ClientAPI.SystemData
    open Microsoft.FSharp.Reflection

    let connect() = 
        let ipadress = IPAddress.Parse("192.168.50.69")
        let endpoint = new IPEndPoint(ipadress, 1113)
        let esSettings = 
            let s = ConnectionSettings.Create()
                        .UseConsoleLogger()
                        .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                        .Build()
            s

        let connection = EventStoreConnection.Create(esSettings, endpoint, null)
        connection.AsyncConnect() |> Async.RunSynchronously
        connection

    let settings = 
        let settings = new JsonSerializerSettings()
        settings.TypeNameHandling <- TypeNameHandling.Auto
        settings

    let serialize (event:'a)= 
        let serializedEvent = JsonConvert.SerializeObject(event, settings)
        let data = System.Text.Encoding.UTF8.GetBytes(serializedEvent)
        let case,_ = FSharpValue.GetUnionFields(event, typeof<'a>)
        EventData(Guid.NewGuid(), case.Name, true, data, null)

    let deserialize<'a> (event: ResolvedEvent) = 
        let serializedString = System.Text.Encoding.UTF8.GetString(event.Event.Data)
        let event = JsonConvert.DeserializeObject<'a>(serializedString, settings)
        event

    let readFromStream (store: IEventStoreConnection) streamId = 
        let slice = store.ReadStreamEventsForwardAsync(streamId, 0, Int32.MaxValue, false).Result

        let events:seq<'a> = 
            slice.Events 
            |> Seq.map deserialize<'a>
            |> Seq.cast 
        
        let nextEventNumber = 
            if slice.IsEndOfStream 
            then None 
            else Some slice.NextEventNumber

        slice.LastEventNumber, (events |> Seq.toList)

    let appendToStream (store:IEventStoreConnection) streamId expectedVersion newEvents =
        let serializedEvents = newEvents |> List.map serialize |> List.toArray
        Async.RunSynchronously <| store.AsyncAppendToStream streamId expectedVersion serializedEvents

