module State

open FsCQRSShop.Contract
open Types
open System

type PersonState = {Id:PersonId; Adresse:Adresse option} 
let initPerson = {Id = PersonId(Guid.Empty); Adresse = None}

type State =
    | Init
    | Person of PersonState
