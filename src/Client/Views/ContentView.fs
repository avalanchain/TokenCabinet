module Client.ContentView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open ClientMsgs
open ClientModels

        
let contentView  (model : Model) (dispatch : Msg -> unit) =
    match model.MenuMediator with 
    | Verification -> VerificationView.view model dispatch
    | PurchaseToken -> PurchaseTokenView.view model dispatch
    | MyInvestments -> MyInvestmentsView.view model dispatch
    | ReferralProgram -> ReferralProgramView.view model dispatch
    | Contacts -> ContactsView.view model dispatch
    | Dashboard -> DashboardView.view