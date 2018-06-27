module Client.ReferralProgramView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable
open Verifications

open ClientMsgs
open ClientModels


let view  (model : Model) (dispatch : AppMsg -> unit) =
    div [ Class "dashboard-wrapper" ]
        [   str "referralProgramView" 
            br []
            str (string model.CurrenciesCurentPrices)
        ]