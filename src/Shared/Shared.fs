namespace Shared

type Counter = int

type ServerError =
    | AuthError
    | InternalError of exn

type ServerResult<'T> = Result<'T, ServerError>

module Route =
    /// Defines how routes are generated on server and mapped from client
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

/// A type that specifies the communication protocol for client and server
/// Every record field must have the type : 'a -> Async<'b> where 'a can also be `unit`
/// Add more such fields, implement them on the server and they be directly available on client
type IAdminProtocol = {   
    getInitCounter  : unit -> Async<Counter> 
    initDb          : unit -> Async<unit> 
}

type ITokenSaleProtocol = {   
    getCryptoCurrencies : unit -> Async<ServerResult<CryptoCurrencies.CryptoCurrency list>> 
}
