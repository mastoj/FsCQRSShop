namespace FsCQRSShop.Domain

module ApplicationBuilder = 
    open Railway
    let buildApplication save handler c = 
        (handler c) >>= save
