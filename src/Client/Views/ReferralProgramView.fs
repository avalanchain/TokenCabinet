module Client.ReferralProgramView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open ClientModelMsg
open Fable
open Verifications

let view  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [   str "referralProgramView" 
            br []
            str (string model.CurrenciesCurentPrices)
        ]