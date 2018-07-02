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
    | LoginFlow of LoginFlowPage
    | Cabinet of CabinetPagePage
//   | Static of Statics.Page
    with static member Default = LoginFlowPage.Default |> LoginFlow
and LoginFlowPage =
    | Login         
    | Register      
    | ForgotPassword
    with static member Default = Login
and CabinetPagePage =
    | PurchaseToken
    | Investments
    | ReferralProgram
    | Verification
    | Contacts
    | Dashboard
    with static member Default = PurchaseToken  

let toHash =
  function
  | MenuPage.Home           -> "#home"
  | MenuPage.LoginFlow lf   -> "#" + (getUnionCaseName lf).ToLowerInvariant()
  | MenuPage.Cabinet tc     -> "#cabinet/" + (getUnionCaseName tc).ToLowerInvariant()
//   | MenuPage.Trading p -> 
//     let uid = match p with 
//                 | Trading.Page.Trader ci
//                 | Trading.Page.VesselOperator ci
//                 | Trading.Page.VesselMaster ci
//                 | Trading.Page.Terminal ci
//                 | Trading.Page.Inspector ci -> "/" + ci.UID.ToString()
//                 | Trading.Page.Archive 
//                 | Trading.Page.All -> ""
//     "#trading/" + getUnionCase(p).Name.ToLowerInvariant() + uid


let goToUrl (e: React.MouseEvent) =
    // e.stopPropagation()
    // e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let loginFlowPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
    allUnionCases<LoginFlowPage>
    |> List.map (fun ed -> map (ed |> MenuPage.LoginFlow) (s ((getUnionCaseName ed).ToLowerInvariant())))


let cabinetPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
    allUnionCases<CabinetPagePage>
    |> List.map (fun ed -> map (ed |> MenuPage.Cabinet) (s "cabinet" </> s ((getUnionCaseName ed).ToLowerInvariant())))


// let [<PassGenerics>]tradingPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
//     let parserWithUid caseName case getName = 
//         map (fun uid -> { Trading.CompanyInfo.UID = uid |> uint32; Trading.CompanyInfo.Name = uid |> uint32 |> getName } |> case |> MenuPage.Trading) (s "trading" </> s caseName </> i32)
//     Trading.pageDefs 
//     |> List.map (fun ed -> 
//                     let pd = ed |> fst
//                     match pd with 
//                     | Trading.Page.Trader ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Trader (fun uid -> (Examples.traders |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.VesselOperator ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.VesselOperator (fun uid -> (Examples.vesselOperators |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.VesselMaster ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.VesselMaster (fun uid -> (Examples.captains |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.Terminal ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Terminal (fun uid -> (Examples.terminals |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.Inspector ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Inspector (fun uid -> (Examples.inspectors |> List.find (fun t -> t.Uid = uid)).Name)
                        
//                     | Trading.Page.Archive 
//                     | Trading.Page.All -> map (pd |> MenuPage.Trading) (s "trading" </> s (getUnionCase(pd).Name.ToLowerInvariant()) ))
                    


/// The URL is turned into a Result.
let pageParser : Parser<MenuPage -> MenuPage, MenuPage> =
    oneOf ([ map MenuPage.Home (s "home") ]
            @ loginFlowPageParsers
            @ cabinetPageParsers
            // @ tradingPageParsers 
            )
    
