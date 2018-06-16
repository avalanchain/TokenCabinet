module Client.ContentView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open ClientModelMsg
        
let contentView  (model : Model) (dispatch : Msg -> unit) =
    match model.MenuMediator with 
    | Verification -> VerificationView.view model dispatch
    | PurchaseToken -> PurchaseTokenView.view model dispatch
    | MyInvestments -> MyInvestmentsView.view model dispatch
    | ReferralProgram -> ReferralProgramView.view model dispatch
    | Contacts -> ContactsView.view model dispatch