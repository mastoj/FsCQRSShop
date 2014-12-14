namespace FsCQRSShop.Web

module WebStart =
    open System
    open System.Web.Http
    open System.Net.Http
    open System.Net
    open FsCQRSShop.Domain
    open FsCQRSShop.Infrastructure.EventStore.EventStore
    open Railway
    open DomainBuilder
    open Model

    let es = connect()

    let map f = 
        match f with 
        | _ -> "Doh!"

    let matchToResult (controller:'T when 'T :> ApiController) res =
        match res with
        | Success events -> controller.Request.CreateResponse(HttpStatusCode.Accepted, events)
        | Failure f -> controller.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, (map f))

    let app controller dto =
        let toStreamId (id:Guid) = sprintf "Test-%O" id
        let appendStream = appendToStream es
        let readStream id = readFromStream es (toStreamId id)
        let save (id, version, events) = 
            appendStream (toStreamId id) version events |> ignore
            Success events
        let deps = {readEvents = readStream; guidGenerator = Guid.NewGuid}

        dto |> toCommand >>= (buildDomainEntry save deps) |> (matchToResult controller)