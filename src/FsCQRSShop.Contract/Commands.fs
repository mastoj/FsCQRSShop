module FsCQRSShop.Contract.Commands
open Types

type Command = 
    PersonCommand of  PersonCommand
and PersonCommand =
    | OpprettFraDsf of PersonId:PersonId * Fødselsnummer:Fødselsnummer * Etternavn:string
    | RegistrerAdresse of PersonId:PersonId * Adresse:Adresse
