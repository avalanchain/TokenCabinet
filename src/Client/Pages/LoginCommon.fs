module Client.LoginCommon
open Shared.Auth


type AuthModel = {
    Token: AuthToken
    UserName: string
}

type LoginState =
    | LoggedOut
    | LoggedIn  of AuthModel
