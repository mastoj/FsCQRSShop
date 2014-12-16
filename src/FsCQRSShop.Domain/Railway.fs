namespace FsCQRSShop.Domain
open System
open FsCQRSShop.Contract.Events


type Dependencies = {readEvents: Guid -> (int*Event list); guidGenerator: unit -> Guid}

module Railway = 
    type Error = 
        | InvalidState of string
        | InvalidStateTransition of string
        | NotSupportedCommand of string
        | UnknownDto of string
        | ValidationError of string
        | InvalidPaymentAmount

    type Result<'T> =
        | Success of 'T
        | Failure of Error

    let bind switchFunction = function
        | Success s -> switchFunction s
        | Failure f -> Failure f
   
    let (>>=) input switchFunction = bind switchFunction input
