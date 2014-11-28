module FsCQRSShop.Domain.Person

open System
open FsCQRSShop.Contract
open Types
open Commands
open Events
open State

let initPerson = {Id = PersonId(Guid.Empty); Adresse = None}

let getSomeData etternavn = {Navn = {Fornavn = "John"; Mellomnavn = Some "Middle"; Etternavn = etternavn}; Adresse = {Linjer = "linjer "}}

let registrerAdresse id adresse = [AdresseRegistrert(id, adresse)]

let handlePerson getPerson state pc =
    match pc with
    | OpprettFraDsf(personId, nummer, etternavn) -> 
        let dsfPerson = getPerson etternavn nummer
        let personOpprettet = PersonOpprettet(personId, nummer, dsfPerson.Navn)
        [personOpprettet; AdresseRegistrert(personId, dsfPerson.Adresse)]
    | RegistrerAdresse(personId, adresse) -> registrerAdresse personId adresse
    | _ -> []

let evolvePerson p event =
    match event with
    | PersonOpprettet(personId, fnr, navn) -> State.Person({Id = personId; Adresse = None} )
    | AdresseRegistrert(personId, adresse) -> State.Person{p with Adresse = Some adresse}
