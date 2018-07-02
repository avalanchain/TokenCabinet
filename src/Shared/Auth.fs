namespace Shared

open System
module Auth =

    type AuthToken = AuthToken of string
    type LoginInfo = { Email: string; Password: string } // TODO: Send password hash only!!!
    type PwdResetInfo = { PwdResetToken: string; Password: string } // TODO: Send password hash only!!!
    type ForgotPasswordInfo = { UserName: string }

    // possible authentication/authorization errors     
    type AuthError = 
       | UserTokenExpired
       | TokenInvalid
       | UserDoesNotHaveAccess

    type ServerError =
        | AuthError of AuthError
        | InternalError of exn
        | NotImplementedError

    type ServerResult<'T> = Result<'T, ServerError>


    // possible errors when logging in
    type LoginFlowServerError = 
        | AccountBanned
        | DdosProtection of blockedForRemaining: TimeSpan
        | LoginInternalError  of exn

    type LoginError = 
        | EmailNotFoundOrPasswordIncorrect
        | LoginServerError of LoginFlowServerError

    type RegisteringError = 
        | EmailAlreadyRegistered
        | ValidationErrors of email: string list * password: string list
        | LoginServerError of LoginFlowServerError

    type PasswordResetError = 
        | LoginServerError of LoginFlowServerError

    type LoginResult         = ServerResult<Result<AuthToken, LoginError>>
    type RegisteringResult   = ServerResult<Result<AuthToken, RegisteringError>>
    type PasswordResetResult = ServerResult<Result<string, PasswordResetError>>

    type ILoginFlowProtocol = {   
        login               : LoginInfo -> Async<LoginResult>
        register            : LoginInfo -> Async<RegisteringResult>
        resetPassword       : ForgotPasswordInfo -> Async<PasswordResetResult>
    }


    // a request with a token
    type SecureRequest<'T> = { Token : AuthToken; Content : 'T }
    type SecureVoidRequest = { Token : AuthToken }
    let secureVoidRequest token = { Token = token }
    let secureRequest token content = { Token = token; Content = content }

    // type BookId = BookId of int
    // // domain model
    // type Book = { Id: BookId; Title: string; (* other propeties *) }

    // // things that could go wrong 
    // // when removing a book from a users wishlist
    // type BookRemovalFromWishlist = 
    //     | BookSuccessfullyRemoved
    //     | BookDoesNotExist

    // // the book store protocol
    // type IBookStoreApi = {
    //     // login to acquire an auth token   
    //     login : LoginInfo -> Async<Result<AuthToken, LoginError>>
    //     // "public" function: no auth needed
    //     searchBooksByTitle : string -> Async<list<Book>> 
    //     // secure function, requires a token
    //     booksOnWishlist : AuthToken -> Async<Result<list<Book>, AuthError>>, 
    //     // secure function, requires a token and a book id
    //     removeBookFromWishlist : SecureRequest<BookId> -> Async<Result<BookRemovalFromWishlist, AuthError>>
    //     // etc . . . 
    // }