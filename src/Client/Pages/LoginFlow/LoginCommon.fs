module Client.LoginCommon
open Shared.Auth


type AuthModel = {
    Token: AuthToken
}

type LoginState =
    | LoggedOut
    | LoggedIn  of AuthModel
