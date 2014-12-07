module FsCQRSShop.Contract.Types
open System

type IWrappedString =
    abstract Value:string
    
type Adresse = {Linjer:string } 

type PersonId = PersonId of Guid 
type PersonNavn = {Fornavn:string; Mellomnavn:string option; Etternavn:string}

let create isValid ctor (s:string) =
    if s = null
    then None
    else
        if isValid s
        then Some (ctor s)
        else None

type Fødselsnummer = Fødselsnummer of string with
    interface IWrappedString with
        member this.Value = 
            let (Fødselsnummer s) = this 
            s

let fødselsnummer = create (fun s -> s <> null && s.Length = 11) Fødselsnummer

type Organisasjonsnummer = Organisasjonsnummer of string with
    interface IWrappedString with
        member this.Value = 
            let (Organisasjonsnummer s) = this 
            s

type DsfPerson = {Navn: PersonNavn; Adresse: Adresse}

let organisasjonsnummer = create (fun s -> s <> null && s.Length = 9) Organisasjonsnummer

type CustomerId = CustomerId of Guid 
type ProductId = ProductId of Guid 
type BasketId = BasketId of Guid 
type OrderId = OrderId of Guid 

type Address = { Street: string }

type OrderLine = {ProductId: Guid; ProductName: string; OriginalPrice: int; DiscountedPrice: int; Quantity: int}
    with override this.ToString() = sprintf "ProdcutName: %s, Price: %d, Discounted: %d, Quantity: %d" this.ProductName this.OriginalPrice this.DiscountedPrice this.Quantity

