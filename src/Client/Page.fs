module Client.Page

open FSharp.Reflection

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Shared.Utils

[<RequireQualifiedAccess>]
type MenuPage = 
    | Home 
    | LoginFlow of LoginFlow
    | Cabinet of CabinetPage
//   | Static of Statics.Page
    with static member Default = LoginFlow.Default |> LoginFlow
and LoginFlow =
    | Login         
    | Register      
    | ForgotPassword
    | PasswordReset
    with static member Default = Login
and CabinetPage =
    | PurchaseToken
    | Investments
    | ReferralProgram
    | Verification
    | Contacts
    // | Dashboard
    with static member Default = PurchaseToken  

let toHash =
  function
  | MenuPage.Home           -> "#home"
  | MenuPage.LoginFlow lf   -> "#" + (getUnionCaseName lf).ToLowerInvariant()
  | MenuPage.Cabinet tc     -> "#cabinet/" + (getUnionCaseName tc).ToLowerInvariant()

let goToUrl (e: React.MouseEvent) =
    // e.stopPropagation()
    // e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let loginFlowPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
    allUnionCases<LoginFlow>
    |> List.map (fun ed -> map (ed |> MenuPage.LoginFlow) (s ((getUnionCaseName ed).ToLowerInvariant())))


let cabinetPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
    allUnionCases<CabinetPage>
    |> List.map (fun ed -> map (ed |> MenuPage.Cabinet) (s "cabinet" </> s ((getUnionCaseName ed).ToLowerInvariant())))


/// The URL is turned into a Result.
let pageParser : Parser<MenuPage -> MenuPage, MenuPage> =
    oneOf ([ map MenuPage.Home (s "home") ]
            @ loginFlowPageParsers
            @ cabinetPageParsers
            )
    
