module FsCQRSShop.Domain.CommandHandling

open FsCQRSShop.Contract
open Commands
open Person

let handle getPerson c state =
    match c with
    | Command.PersonCommand(pc) -> handlePerson getPerson state pc
