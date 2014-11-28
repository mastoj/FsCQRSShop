module FsCQRSShop.Tests
open System
open Xunit
open FsUnit.Xunit

open FsCQRSShop.Contract
open Events
open Commands
open Types

open FsCQRSShop.Domain
open EventHandling
open CommandHandling
open State

type Dependencies = {GetPerson: (string -> Fødselsnummer -> DsfPerson) option}
type TestSpec = {PreCondition: (Event list * Dependencies option); Action: Command; PostCondition: Event list}

let Given (events: Event list, dependencies: Dependencies option) = events, dependencies
let When (command: Command) (events: Event list, dependencies: Dependencies option) = events, dependencies, command
let Expect (expectedEvents:Event list) (events:Event list, dependencies: Dependencies option, command: Command) = 
    let defaultGetPerson (x:string) (y:Fødselsnummer) = {Navn = {Fornavn = "Tomas"; Mellomnavn = None; Etternavn = "Jansson"}; Adresse = {Linjer = "Linje 1"}}
    let getPerson = match dependencies with 
                    | Some dep -> match dep.GetPerson with
                                    | Some f -> f
                                    | None -> defaultGetPerson
                    | None -> defaultGetPerson
    evolve command events |> handle getPerson command |> should equal expectedEvents

[<Fact>]
let ``Created game should be created``() =
    Given ([], None)
    |> When (Command.PersonCommand(OpprettFraDsf(PersonId(Guid.Empty), (fødselsnummer "08080812345") |> Option.get, "jansson")))
    |> Expect [PersonOpprettet(PersonId(Guid.Empty), (fødselsnummer "08080812345") |> Option.get, {Fornavn = "Tomas"; Mellomnavn = None; Etternavn = "Jansson"});
                AdresseRegistrert(PersonId(Guid.Empty), {Linjer = "Linje 1"}) ]
