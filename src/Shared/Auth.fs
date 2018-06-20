namespace Shared

module Auth =

    type AuthToken = AuthToken of string
    type LoginInfo = { Username: string; Password: string }

    // possible errors when logging in
    type LoginError = 
        | UserDoesNotExist
        | PasswordIncorrect
        | AccountBanned

    // a request with a token
    type SecureRequest<'T> = { Token : AuthToken; Content : 'T }
        with static member Unit token = { Token = token; Content = () }

    // possible authentication/authorization errors     
    type AuthError = 
       | UserTokenExpired
       | TokenInvalid
       | UserDoesNotHaveAccess

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