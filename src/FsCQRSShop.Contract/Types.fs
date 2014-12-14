module FsCQRSShop.Contract.Types
open System

type CustomerId = CustomerId of Guid 
type ProductId = ProductId of Guid 
type BasketId = BasketId of Guid 
type OrderId = OrderId of Guid 

type Address = { Street: string }

type OrderLine = {ProductId: ProductId; ProductName: string; OriginalPrice: int; DiscountedPrice: int; Quantity: int}
    with override this.ToString() = sprintf "ProdcutName: %s, Price: %d, Discounted: %d, Quantity: %d" this.ProductName this.OriginalPrice this.DiscountedPrice this.Quantity

