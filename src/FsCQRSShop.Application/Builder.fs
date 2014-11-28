module FsCQRSShop.Application.Builder

open FsCQRSShop.Contract
open FsCQRSShop.Domain
open Types
open Person
open Commands
open CommandHandling
open EventHandling

let toStreamId id = sprintf "Data-%O" id

let createApplication readStream appendStream = 
    let aggId command = 
        match command with
        | PersonCommand(OpprettFraDsf(PersonId id, _, _)) -> id
        | PersonCommand(RegistrerAdresse(PersonId id, _)) -> id

    let loadState command id = 
        let streamId = toStreamId id
        let (version, events) = readStream streamId
        let state = evolve command events
        (version, state)
        
    let save id expectedVersion newEvents =
        let streamId = toStreamId id
        appendStream streamId expectedVersion newEvents

    let getPerson (x:string) (y:Fødselsnummer) = {Navn = {Fornavn = "Tomas"; Mellomnavn = None; Etternavn = "Jansson"}; Adresse = {Linjer = "Linje 1"}}
    fun command ->
        let aggregateId = aggId command
        let state = loadState command aggregateId 
        state ||> fun version state -> (version, handle getPerson command state) ||> save aggregateId 
