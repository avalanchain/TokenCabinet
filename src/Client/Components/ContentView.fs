module Client.ContentView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open Fable
open Verifications

let verificationView  = Verification.verificationView


let purchaseTokenView = PurchaseToken.purchaseTokenView
        
let myInvestmentsView  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [ str "myInvestmentsView" ]
let referralProgramView  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [   str "referralProgramView" 
            br []
            str (string model.CurrenciesCurentPrices)
        ]

let contactsView  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [ str "Contacts" ]
        
let contentView  (model : Model) (dispatch : Msg -> unit) =
    match model.MenuMediator with 
    | Verification -> verificationView model dispatch
    | PurchaseToken -> purchaseTokenView model dispatch
    | MyInvestments -> myInvestmentsView model dispatch
    | ReferralProgram -> referralProgramView model dispatch
    | Contacts -> contactsView model dispatch