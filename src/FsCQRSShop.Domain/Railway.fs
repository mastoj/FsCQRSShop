namespace FsCQRSShop.Domain

module Railway = 
    type Error = 
        | InvalidState of string
        | InvalidStateTransition of string
        | NotSupportedCommand of string

    type Result<'T> =
        | Success of 'T
        | Failure of Error

    let bind switchFunction = 
        fun input -> match input with
                        | Success s -> switchFunction s
                        | Failure s -> Failure s

    let (>>=) input switchFunction = bind switchFunction input
