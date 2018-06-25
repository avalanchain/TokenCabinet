module Client.DashboardView

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientMsgs
open JsInterop


// let view  =
//   div []
//       []
let view  =


         div [ Class "row wrapper wrapper-content" ]
            [ div [ Class "col-lg-4" ]
                [ div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str "Colors buttons" ]
                          div [ HTMLAttr.Custom ("ibox-tools", "") ]
                            [ ] ]
                      div [ Class "ibox-content" ]
                        [ p [ ]
                            [ str "Use any of the available button classes to quickly create a styled button." ]
                          h3 [ Class "font-bold" ]
                            [ str "Normal buttons" ]
                          p [ ]
                            [ button [ Type "button"
                                       Class "btn btn-w-m btn-default" ]
                                [ str "Default" ]
                              button [ Type "button"
                                       Class "btn btn-w-m btn-primary" ]
                                [ str "Primary" ]
                              button [ Type "button"
                                       Class "btn btn-w-m btn-success" ]
                                [ str "Success" ]
                              button [ Type "button"
                                       Class "btn btn-w-m btn-info" ]
                                [ str "Info" ]
                              button [ Type "button"
                                       Class "btn btn-w-m btn-warning" ]
                                [ str "Warning" ]
                              button [ Type "button"
                                       Class "btn btn-w-m btn-danger" ]
                                [ str "Danger" ]
                              button [ Type "button"
                                       Class "btn btn-w-m btn-white" ]
                                [ str "Danger" ]
                              button [ Type "button"
                                       Class "btn btn-w-m btn-link" ]
                                [ str "Link" ] ] ] ] ]
              div [ Class "col-lg-4" ]
                [ div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str "Diferent size" ]
                          div [ HTMLAttr.Custom ("ibox-tools", "") ]
                            [ ] ]
                      div [ Class "ibox-content" ]
                        [ 
                            
                          h3 [ Class "font-bold" ]
                            [ str "Button Sizes" ]
                          p [ ]
                            [ button [ Type "button"
                                       Class "btn btn-primary btn-lg" ]
                                [ str "Large button" ]
                              button [ Type "button"
                                       Class "btn btn-default btn-lg" ]
                                [ str "Large button" ]
                              br [ ]
                              button [ Type "button"
                                       Class "btn btn-primary" ]
                                [ str "Default button" ]
                              button [ Type "button"
                                       Class "btn btn-default" ]
                                [ str "Default button" ]
                              br [ ]
                              button [ Type "button"
                                       Class "btn btn-primary btn-sm" ]
                                [ str "Small button" ]
                              button [ Type "button"
                                       Class "btn btn-default btn-sm" ]
                                [ str "Small button" ]
                              br [ ]
                              button [ Type "button"
                                       Class "btn btn-primary btn-xs" ]
                                [ str "Mini button" ]
                              button [ Type "button"
                                       Class "btn btn-default btn-xs" ]
                                [ str "Mini button" ] ] ] ] ]
              div [ Class "col-lg-4" ]
                [ div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str "Outline and Block Buttons" ]
                          div [ HTMLAttr.Custom ("ibox-tools", "") ]
                            [ ] ]
                      div [ Class "ibox-content" ]
                        [ 
                          h3 [ Class "font-bold" ]
                            [ str "Outline buttons" ]
                          p [ ]
                            [ button [ Type "button"
                                       Class "btn btn-outline btn-default" ]
                                [ str "Default" ]
                              button [ Type "button"
                                       Class "btn btn-outline btn-primary" ]
                                [ str "Primary" ]
                              button [ Type "button"
                                       Class "btn btn-outline btn-success" ]
                                [ str "Success" ]
                              button [ Type "button"
                                       Class "btn btn-outline btn-info" ]
                                [ str "Info" ]
                              button [ Type "button"
                                       Class "btn btn-outline btn-warning" ]
                                [ str "Warning" ]
                              button [ Type "button"
                                       Class "btn btn-outline btn-danger" ]
                                [ str "Danger" ]
                              button [ Type "button"
                                       Class "btn btn-outline btn-link" ]
                                [ str "Link" ] ]
                          h3 [ Class "font-bold" ]
                            [ str "Block buttons" ]
                          p [ ]
                            [ button [ Type "button"
                                       Class "btn btn-block btn-outline btn-primary" ]
                                [ str "Primary" ] ] ] ] ]
              div [ Class "col-lg-12" ]
                [ div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str "3D Buttons" ]
                          div [ HTMLAttr.Custom ("ibox-tools", "") ]
                            [ ] ]
                      div [ Class "ibox-content" ]
                        [ 
                          h3 [ Class "font-bold" ]
                            [ str "Three diminsion button" ]
                          button [ Class "btn btn-primary dim btn-large-dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-money" ]
                                [ ] ]
                          button [ Class "btn btn-warning dim btn-large-dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-warning" ]
                                [ ] ]
                          button [ Class "btn btn-danger  dim btn-large-dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-heart" ]
                                [ ] ]
                          
                          button [ Class "btn btn-info  dim btn-large-dim btn-outline"
                                   Type "button" ]
                            [ i [ Class "fa fa-ruble" ]
                                [ ] ]
                          button [ Class "btn btn-primary dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-money" ]
                                [ ] ]
                          button [ Class "btn btn-warning dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-warning" ]
                                [ ] ]
                          button [ Class "btn btn-primary dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-check" ]
                                [ ] ]
                          button [ Class "btn btn-success  dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-upload" ]
                                [ ] ]
                          button [ Class "btn btn-info  dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-paste" ]
                                [ ] ]
                          button [ Class "btn btn-warning  dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-warning" ]
                                [ ] ]
                          button [ Class "btn btn-default  dim "
                                   Type "button" ]
                            [ i [ Class "fa fa-star" ]
                                [ ] ]
                          button [ Class "btn btn-danger  dim "
                                   Type "button" ]
                            [ i [ Class "fa fa-heart" ]
                                [ ] ]
                          button [ Class "btn btn-outline btn-primary dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-money" ]
                                [ ] ]
                          button [ Class "btn btn-outline btn-warning dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-warning" ]
                                [ ] ]
                          button [ Class "btn btn-outline btn-primary dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-check" ]
                                [ ] ]
                          button [ Class "btn btn-outline btn-success  dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-upload" ]
                                [ ] ]
                          button [ Class "btn btn-outline btn-info  dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-paste" ]
                                [ ] ]
                          button [ Class "btn btn-outline btn-warning  dim"
                                   Type "button" ]
                            [ i [ Class "fa fa-warning" ]
                                [ ] ]
                          button [ Class "btn btn-outline btn-danger  dim "
                                   Type "button" ]
                            [ i [ Class "fa fa-heart" ]
                                [ ] ] ] ] ]
              div [ Class "col-lg-12" ]
                [ div [ Class "row" ]
                    [ div [ Class "col-lg-6" ]
                        [ div [ Class "ibox float-e-margins" ]
                            [ div [ Class "ibox-title" ]
                                [ h5 [ ]
                                    [ str "Button dropdowns" ]
                                  div [ HTMLAttr.Custom ("ibox-tools", "") ]
                                    [ ] ]
                              div [ Class "ibox-content" ]
                                [ p [ ]
                                    [ str "Droppdowns buttons are avalible with any color and any size." ]
                                  h3 [ Class "font-bold" ]
                                    [ str "Dropdowns / Icons" ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-primary"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-warning"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-default"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  br [ ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-primary btn-sm"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-warning btn-sm"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-default btn-sm"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  br [ ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-primary btn-xs"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-warning btn-xs"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  div [ Class "btn-group"
                                        HTMLAttr.Custom ("uib-dropdown", "") ]
                                    [ button [ Type "button"
                                               Class "btn btn-default btn-xs"
                                               HTMLAttr.Custom ("uib-dropdown-toggle", "") ]
                                        [ str "Action"
                                          span [ Class "caret" ]
                                            [ ] ]
                                      ul [ Role "menu"
                                           HTMLAttr.Custom ("uib-dropdown-menu", "") ]
                                        [ li [ ]
                                            [ a [ Href "" ]
                                                [ str "Action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Another action" ] ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Something else here" ] ]
                                          li [ Class "divider" ]
                                            [ ]
                                          li [ ]
                                            [ a [ Href "" ]
                                                [ str "Separated link" ] ] ] ]
                                  p [ ]
                                    [ str "To buttons with any color or any size you can add extra icon on the left or the right side." ]
                                  h3 [ Class "font-bold" ]
                                    [ str "Commom Icon Buttons" ]
                                  p [ ]
                                    [ 
                                      button [ Class "btn btn-warning "
                                               Type "button" ]
                                        [ i [ Class "fa fa-warning" ]
                                            [ ]
                                          span [ Class "bold" ]
                                            [ str "Warning" ] ]
                                      a [ Class "btn btn-white btn-bitbucket" ]
                                        [ i [ Class "fa fa-user-md" ]
                                            [ ] ]
                                      a [ Class "btn btn-white btn-bitbucket" ]
                                        [ i [ Class "fa fa-group" ]
                                            [ ] ]
                                      a [ Class "btn btn-white btn-bitbucket" ]
                                        [ i [ Class "fa fa-wrench" ]
                                            [ ] ]
                                      a [ Class "btn btn-white btn-bitbucket" ]
                                        [ i [ Class "fa fa-exchange" ]
                                            [ ] ]
                                      a [ Class "btn btn-white btn-bitbucket" ]
                                        [ i [ Class "fa fa-check-circle-o" ]
                                            [ ] ]
                                      a [ Class "btn btn-white btn-bitbucket" ]
                                        [ i [ Class "fa fa-road" ]
                                            [ ] ]
                                      a [ Class "btn btn-white btn-bitbucket" ]
                                        [ i [ Class "fa fa-ambulance" ]
                                            [ ] ]
                                       ] ] ] ]
                      div [ Class "col-lg-6" ]
                        [ div [ Class "ibox float-e-margins" ]
                            [ div [ Class "ibox-title" ]
                                [ h5 [ ]
                                    [ str "Grouped Buttons / Pagination" ]
                                  div [ HTMLAttr.Custom ("ibox-tools", "") ]
                                    [ ] ]
                              div [ Class "ibox-content" ]
                                [ p [ ]
                                    [ str "This is a group of buttons, ideal for sytuation where many actions are related to same element." ]
                                  h3 [ Class "font-bold" ]
                                    [ str "Button Group" ]
                                  div [ Class "btn-group" ]
                                    [ button [ Class "btn btn-white"
                                               Type "button" ]
                                        [ str "Left" ]
                                      button [ Class "btn btn-primary"
                                               Type "button" ]
                                        [ str "Middle" ]
                                      button [ Class "btn btn-white"
                                               Type "button" ]
                                        [ str "Right" ] ]
                                  br [ ]
                                  h3 [ ]
                                    [ str "Pagination" ]
                                  
                                  pre [ ]
                                    [ str "Page: {{main.bigCurrentPage}}" ] ] ] ] ]
                  div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str "Toggle Buttons" ]
                          div [ HTMLAttr.Custom ("ibox-tools", "") ]
                            [ ] ]
                      div [ Class "ibox-content" ]
                        [ h3 [ Class "font-bold" ]
                            [ str "Toggle buttons Variations" ]
                          div [ Class "row" ]
                            [ div [ Class "col-lg-6" ]
                                [ p [ ]
                                    [ str "Button groups can act as a radio or a switch or even a single toggle. Below are some examples click to see what happens" ]
                                  h4 [ ]
                                    [ str "Single toggle" ]
                                  pre [ ]
                                    [ str "{{main.singleModel}}" ]
                                  button [ Type "button"
                                           Class "btn btn-primary"
                                           HTMLAttr.Custom ("ng-model", "main.singleModel")
                                           HTMLAttr.Custom ("uib-btn-checkbox", "")
                                           HTMLAttr.Custom ("uib-btn-checkbox-true", "1")
                                           HTMLAttr.Custom ("uib-btn-checkbox-false", "0") ]
                                    [ str "Single Toggle" ]
                                  h4 [ ]
                                    [ str "Checkbox" ]
                                  pre [ ]
                                    [ str "{{main.checkModel}}" ]
                                  div [ Class "btn-group" ]
                                    [ label [ Class "btn btn-primary"
                                              HTMLAttr.Custom ("ng-model", "main.checkModel.left")
                                              HTMLAttr.Custom ("uib-btn-checkbox", "") ]
                                        [ str "Left" ]
                                      label [ Class "btn btn-primary"
                                              HTMLAttr.Custom ("ng-model", "main.checkModel.middle")
                                              HTMLAttr.Custom ("uib-btn-checkbox", "") ]
                                        [ str "Middle" ]
                                      label [ Class "btn btn-primary"
                                              HTMLAttr.Custom ("ng-model", "main.checkModel.right")
                                              HTMLAttr.Custom ("uib-btn-checkbox", "") ]
                                        [ str "Right" ] ] ]
                              div [ Class "col-lg-6" ]
                                [ h4 [ ]
                                    [ str "Radio &amp; Uncheckable Radio" ]
                                  pre [ ]
                                    [ str "{{main.radioModel || 'null'}}" ]
                                  div [ Class "btn-group" ]
                                    [ label [ Class "btn btn-primary"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Left'") ]
                                        [ str "Left" ]
                                      label [ Class "btn btn-primary"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Middle'") ]
                                        [ str "Middle" ]
                                      label [ Class "btn btn-primary"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Right'") ]
                                        [ str "Right" ] ]
                                  div [ Class "btn-group" ]
                                    [ label [ Class "btn btn-success"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Left'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Left" ]
                                      label [ Class "btn btn-success"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Middle'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Middle" ]
                                      label [ Class "btn btn-success"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Right'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Right" ] ]
                                  div [ Class "btn-group" ]
                                    [ label [ Class "btn btn-warning"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Left'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Left" ]
                                      label [ Class "btn btn-warning"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Middle'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Middle" ]
                                      label [ Class "btn btn-warning"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Right'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Right" ] ]
                                  div [ Class "btn-group" ]
                                    [ label [ Class "btn btn-default"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Left'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Left" ]
                                      label [ Class "btn btn-default"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Middle'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Middle" ]
                                      label [ Class "btn btn-default"
                                              HTMLAttr.Custom ("ng-model", "main.radioModel")
                                              HTMLAttr.Custom ("uib-btn-radio", "'Right'")
                                              HTMLAttr.Custom ("uncheckable", "") ]
                                        [ str "Right" ] ] ] ] ] ] ]
              div [ Class "col-lg-12" ]
                [ div [ Class "row" ]
                    [ div [ Class "col-lg-6" ]
                        [ div [ Class "ibox float-e-margins" ]
                            [ div [ Class "ibox-title" ]
                                [ h5 [ ]
                                    [ str "Circle Icon Buttons" ]
                                  div [ HTMLAttr.Custom ("ibox-tools", "") ]
                                    [ ] ]
                              div [ Class "ibox-content" ]
                                [ 
                                  h3 [ Class "font-bold" ]
                                    [ str "Circle buttons" ]
                                  br [ ]
                                  button [ Class "btn btn-default btn-circle"
                                           Type "button" ]
                                    [ i [ Class "fa fa-check" ]
                                        [ ] ]
                                  button [ Class "btn btn-primary btn-circle"
                                           Type "button" ]
                                    [ i [ Class "fa fa-list" ]
                                        [ ] ]
                                  button [ Class "btn btn-success btn-circle"
                                           Type "button" ]
                                    [ i [ Class "fa fa-link" ]
                                        [ ] ]
                                  button [ Class "btn btn-info btn-circle"
                                           Type "button" ]
                                    [ i [ Class "fa fa-check" ]
                                        [ ] ]
                                  button [ Class "btn btn-warning btn-circle"
                                           Type "button" ]
                                    [ i [ Class "fa fa-times" ]
                                        [ ] ]
                                  button [ Class "btn btn-danger btn-circle"
                                           Type "button" ]
                                    [ i [ Class "fa fa-heart" ]
                                        [ ] ]
                                  button [ Class "btn btn-danger btn-circle btn-outline"
                                           Type "button" ]
                                    [ i [ Class "fa fa-heart" ]
                                        [ ] ]
                                  br [ ]
                                  br [ ]
                                  button [ Class "btn btn-default btn-circle btn-lg"
                                           Type "button" ]
                                    [ i [ Class "fa fa-check" ]
                                        [ ] ]
                                  button [ Class "btn btn-primary btn-circle btn-lg"
                                           Type "button" ]
                                    [ i [ Class "fa fa-list" ]
                                        [ ] ]
                                  button [ Class "btn btn-success btn-circle btn-lg"
                                           Type "button" ]
                                    [ i [ Class "fa fa-link" ]
                                        [ ] ]
                                  button [ Class "btn btn-info btn-circle btn-lg"
                                           Type "button" ]
                                    [ i [ Class "fa fa-check" ]
                                        [ ] ]
                                  button [ Class "btn btn-warning btn-circle btn-lg"
                                           Type "button" ]
                                    [ i [ Class "fa fa-times" ]
                                        [ ] ]
                                  button [ Class "btn btn-danger btn-circle btn-lg"
                                           Type "button" ]
                                    [ i [ Class "fa fa-heart" ]
                                        [ ] ]
                                  button [ Class "btn btn-danger btn-circle btn-lg btn-outline"
                                           Type "button" ]
                                    [ i [ Class "fa fa-heart" ]
                                        [ ] ] ] ] ]
                      div [ Class "col-lg-6" ]
                        [ div [ Class "ibox float-e-margins" ]
                            [ div [ Class "ibox-title" ]
                                [ h5 [ ]
                                    [ str "Rounded Buttons" ]
                                  div [ HTMLAttr.Custom ("ibox-tools", "") ]
                                    [ ] ]
                              div [ Class "ibox-content" ]
                                [ 
                                  h3 [ Class "font-bold" ]
                                    [ str "Button Group" ]
                                  p [ ]
                                    [ a [ Class "btn btn-default btn-rounded"
                                          Href "" ]
                                        [ str "Default" ]
                                      a [ Class "btn btn-primary btn-rounded"
                                          Href "" ]
                                        [ str "Primary" ]
                                      a [ Class "btn btn-success btn-rounded"
                                          Href "" ]
                                        [ str "Success" ]
                                      a [ Class "btn btn-info btn-rounded"
                                          Href "" ]
                                        [ str "Info" ]
                                      a [ Class "btn btn-warning btn-rounded"
                                          Href "" ]
                                        [ str "Warning" ]
                                      a [ Class "btn btn-danger btn-rounded"
                                          Href "" ]
                                        [ str "Danger" ]
                                      a [ Class "btn btn-danger btn-rounded btn-outline"
                                          Href "" ]
                                        [ str "Danger" ]
                                      br [ ]
                                      br [ ]
                                      ] ] ] ] ] ] ]