namespace FsCQRSShop.Domain
open System
open FsCQRSShop.Contract.Events

module DomainBuilder = 
    open Railway
    open CommandHandling

    let buildDomainEntry save (deps:Dependencies) c = 
        (handle deps c) >>= save
