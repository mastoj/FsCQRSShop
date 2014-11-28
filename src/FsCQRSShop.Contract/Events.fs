module FsCQRSShop.Contract.Events

open Types

type Event = 
    | PersonOpprettet of PersonId:PersonId * Fødselsnummer:Fødselsnummer * Navn:PersonNavn
    | AdresseRegistrert of PersonId:PersonId * Adresse:Adresse

