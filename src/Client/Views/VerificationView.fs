module Client.VerificationView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open Fable

let view (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [ str "verificationView" 
          div [ Class "steps"
                Id "stepsDemo" ]
            [ div [ Class "step-item is-active is-success" ]
                [ div [ Class "step-marker" ]
                    [ str "1" ]
                  div [ Class "step-details" ]
                    [ p [ Class "step-title" ]
                        [ str "Account" ] ] ]
              div [ Class "step-item" ]
                [ div [ Class "step-marker" ]
                    [ str "2" ]
                  div [ Class "step-details" ]
                    [ p [ Class "step-title" ]
                        [ str "Profile" ] ] ]
              div [ Class "step-item" ]
                [ div [ Class "step-marker" ]
                    [ str "3" ]
                  div [ Class "step-details" ]
                    [ p [ Class "step-title" ]
                        [ str "Social" ] ] ]
              div [ Class "step-item" ]
                [ div [ Class "step-marker" ]
                    [ str "4" ]
                  div [ Class "step-details" ]
                    [ p [ Class "step-title" ]
                        [ str "Finish" ] ] ]
              div [ Class "steps-content" ]
                [ div [ Class "step-content has-text-centered is-active" ]
                    [ div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Username" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control" ]
                                    [ input [ Class "input"
                                              Name "username"
                                              Id "username"
                                              Type "text"
                                              Placeholder "Username"
                                              AutoFocus true
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ]
                      div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Password" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control has-icon has-icon-right" ]
                                    [ input [ Class "input"
                                              Type "password"
                                              Name "password"
                                              Id "password"
                                              Placeholder "Password"
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ]
                      div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Confirm password" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control has-icon has-icon-right" ]
                                    [ input [ Class "input"
                                              Type "password"
                                              Name "password_confirm"
                                              Id "password_confirm"
                                              Placeholder "Confirm password"
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ] ]
                  div [ Class "step-content has-text-centered" ]
                    [ div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Firstname" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control" ]
                                    [ input [ Class "input"
                                              Name "firstname"
                                              Id "firstname"
                                              Type "text"
                                              Placeholder "Firstname"
                                              AutoFocus true
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ]
                      div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Last name" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control has-icon has-icon-right" ]
                                    [ input [ Class "input"
                                              Type "text"
                                              Name "lastname"
                                              Id "lastname"
                                              Placeholder "Last name"
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ]
                      div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Email" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control has-icon has-icon-right" ]
                                    [ input [ Class "input"
                                              Type "email"
                                              Name "email"
                                              Id "email"
                                              Placeholder "Email"
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ] ]
                  div [ Class "step-content has-text-centered" ]
                    [ div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Facebook account" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control" ]
                                    [ input [ Class "input"
                                              Name "facebook"
                                              Id "facebook"
                                              Type "text"
                                              Placeholder "Facebook account url"
                                              AutoFocus true
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ]
                      div [ Class "field is-horizontal" ]
                        [ div [ Class "field-label is-normal" ]
                            [ label [ Class "label" ]
                                [ str "Twitter account" ] ]
                          div [ Class "field-body" ]
                            [ div [ Class "field" ]
                                [ div [ Class "control" ]
                                    [ input [ Class "input"
                                              Name "twitter"
                                              Id "twitter"
                                              Type "text"
                                              Placeholder "Twitter account url"
                                              AutoFocus true
                                              HTMLAttr.Custom ("data-validate", "require") ] ] ] ] ] ]
                  div [ Class "step-content has-text-centered" ]
                    [ h1 [ Class "title is-4" ]
                        [ str "Your account is now created!" ] ] ]
              div [ Class "steps-actions" ]
                [ div [ Class "steps-action" ]
                    [ a [ Href "#"
                          HTMLAttr.Custom ("data-nav", "previous")
                          Class "button is-light" ]
                        [ str "Previous" ] ]
                  div [ Class "steps-action" ]
                    [ a [ Href "#"
                          HTMLAttr.Custom ("data-nav", "next")
                          Class "button is-light" ]
                        [ str "Next" ] ] ] ]
        ]
        

