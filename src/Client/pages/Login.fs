module Client.Login

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open Elmish

open ServerCode.Domain
open ServerCode.EntityList

open Entity
open Style
open Messages
    
type LoginState =
| LoggedOut
| LoggedIn of JWT

type Model = { 
    State : LoginState
    Login : Login
    ErrorMsg : string }

let authUser (login:Login,apiUrl) =
    promise {
        if String.IsNullOrEmpty login.UserName then return! failwithf "You need to fill in a username." else
        if String.IsNullOrEmpty login.Password then return! failwithf "You need to fill in a password." else

        let body = toJson login

        let props = 
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]
        
        try

            let! response = Fetch.fetch apiUrl props

            if not response.Ok then
                return! failwithf "Error: %d" response.Status
            else    
                let! data = response.text() 
                return data
        with
        | _ -> return! failwithf "Could not authenticate user."
    }

let authUserCmd (login: Login) apiUrl = 
    //Cmd.ofPromise authUser (login,apiUrl) GetTokenSuccess AuthError // TODO: Restore server call
    if login.UserName = "test" && login.Password = "test" then Cmd.ofMsg(GetTokenSuccess "test") 
    else Cmd.ofMsg(AuthError (Exception "Could not authenticate user.")) 

let init (user: UserData option) = 
    match user with
    | None ->
        { Login = { UserName = ""; Password = ""}
          State = LoggedOut
          ErrorMsg = "" }, Cmd.none
    | Some user ->
        { Login = { UserName = user.UserName; Password = ""}
          State = LoggedIn user.Token
          ErrorMsg = "" }, Cmd.none

let update (msg: LoginMsg) model : Model * Cmd<LoginMsg> = 
    match msg with
    | GetTokenSuccess token ->
        { model with State = LoggedIn token;  Login = { model.Login with Password = "" } }, []
    | SetUserName name ->
        { model with Login = { model.Login with UserName = name; Password = "" }}, []
    | SetPassword pw ->
        { model with Login = { model.Login with Password = pw }}, []
    | ClickLogIn ->
        model, authUserCmd model.Login "/api/users/login"
    | AuthError exn ->
        { model with ErrorMsg = string (exn.Message) }, []

let [<Literal>] ENTER_KEY = 13.

let view model (dispatch: AppMsg -> unit) = 
    let showErrorClass = if String.IsNullOrEmpty model.ErrorMsg then "hidden" else ""
    let buttonActive = if String.IsNullOrEmpty model.Login.UserName || String.IsNullOrEmpty model.Login.Password then "btn-disabled" else "btn-primary"

    let onEnter msg dispatch =
        function 
        | (ev:React.KeyboardEvent) when ev.keyCode = ENTER_KEY ->
            ev.preventDefault() 
            dispatch msg
        | _ -> ()
        |> OnKeyDown
        
    match model.State with
    | LoggedIn _ ->
        div [Id "greeting"] [
          h3 [ ClassName "text-center" ] [ str (sprintf "Hi %s!" model.Login.UserName) ]
        ]

    | LoggedOut ->
        div [ ClassName "app flex-row align-items-center" ] [
            div [ ClassName "container" ] [
                div [ ClassName "row justify-content-center" ] [
                    div [ ClassName "col-md-8" ] [
                        div [ ClassName "card-group mb-0" ] [
                            div [ ClassName "card p-4" ] [
                                div [ ClassName "card-block" ] [
                                    h1 [] [ text "Login" ]
                                    p [ ClassName "text-muted" ] [ text "Log in with 'test' / 'test'." ]
                                    div [ ClassName "input-group mb-3" ] [
                                        span [ ClassName "input-group-addon" ] [ i [ ClassName "icon-user" ][]]
                                        input [ Id "username"
                                                Type "text" 
                                                ClassName "form-control"
                                                Placeholder "Username" 
                                                DefaultValue (U2.Case1 model.Login.UserName)
                                                OnChange (fun ev -> dispatch (LoginMsg (SetUserName !!ev.target?value)))
                                                AutoFocus true
                                                ]
                                    ]
                                    div [ ClassName "input-group mb-4" ] [
                                        span [ ClassName "input-group-addon" ] [ i [ ClassName "icon-lock" ][]]
                                        input [ Type "password" 
                                                ClassName "form-control" 
                                                Placeholder "Password"  
                                                DefaultValue (U2.Case1 model.Login.Password)
                                                OnChange (fun ev -> dispatch (LoginMsg (SetPassword !!ev.target?value)))
                                                onEnter (LoginMsg ClickLogIn) dispatch
                                                ]
                                    ]
                                    div [ ClassName "row" ] [
                                        div [ ClassName "col-6" ] [
                                            button [ Type "button" 
                                                     ClassName "btn btn-primary px-4" 
                                                     OnClick (fun _ -> dispatch (LoginMsg ClickLogIn)) 
                                                     ] [ text "Login" ]
                                        ]
                                        div [ ClassName "col-6 text-right" ] [
                                            button [ Type "button" 
                                                     ClassName "btn btn-link px-0" ] [ text "Forgot password?" ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

