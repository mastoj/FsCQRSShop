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

    let bind switchFunction = 
        fun input -> match input with
                        | Success s -> switchFunction s
                        | Failure s -> Failure s

    let (>>=) input switchFunction = bind switchFunction input
