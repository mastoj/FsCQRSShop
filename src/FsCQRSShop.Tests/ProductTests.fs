namespace FsCQRSShop.Tests.ProductTests
open System
open Xunit
open FsUnit.Xunit

open FsCQRSShop.Contract
open Events
open Commands
open Types

open FsCQRSShop.Domain

open FsCQRSShop.Tests.Specification
open FsCQRSShop.Domain.Railway

module ``When creating a product`` = 
    [<Fact>]
    let ``the product should be created``() =
        let id = Guid.NewGuid()
        Given ([], None)
        |> When (Command.ProductCommand(CreateProduct(ProductId id, "Honey", 80)))
        |> Expect [ProductCreated(ProductId id, "Honey", 80)]

    [<Fact>]
    let ``it should fail if id is not unique``() =
        let id = Guid.NewGuid()
        Given ([(id, [ProductCreated(ProductId id, "Honey", 80)])], None)
        |> When (Command.ProductCommand(CreateProduct(ProductId id, "Honey", 80)))
        |> ExpectFail (InvalidState ("Product"))