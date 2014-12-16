namespace FsCQRSShop.Domain

module DomainBuilder = 
    open System
    open FsCQRSShop.Contract.Events
    open FsCQRSShop.Contract.Types
    open FsCQRSShop.Contract.Commands
    open Railway
    open CommandHandling


    let validateCommand = function
        | Command.BasketCommand(CheckoutBasket(id, addr)) -> 
            match addr.Street.Trim() with
            | "" -> Failure (ValidationError "Invalid address")
            | trimmed -> Success (BasketCommand(CheckoutBasket(id, {addr with Street = trimmed})))
        | c -> Success c

    let buildDomainEntry save deps c = 
        (validateCommand c) >>= (handle deps) >>= save
