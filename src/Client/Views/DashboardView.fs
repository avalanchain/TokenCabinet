module Client.DashboardView

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open JsInterop

importAll "../lib/css/dashboard.css"

let view  =
    div [ Id "dashboard-wrapper" 
          Class "columns" ]
        [ div [ Class "column" ]
            [ ]
          div [ Class "content column is-11" ]
            [ nav [ Class "breadcrumbs" ]
                [ ul [ ]
                    [ li [ ]
                        [ a [ Href "#" ]
                            [ str "Home" ] ]
                      li [ ]
                        [ str "Dashboard" ] ] ]
              div [ Class "dashboard-wrapper" ]
                [ div [ Class "columns" ]
                    [ div [ Class "column" ]
                        [ div [ Id "main-dashboard"
                                Class "section-wrapper" ]
                            [ div [ Class "columns dashboard-columns" ]
                                [ div [ Class "column is-3" ]
                                    [ div [ Class "flex-card card-overflow light-bordered light-raised" ]
                                        [ div [ Class "icon-header" ]
                                            [ i [ Class "material-icons primary" ]
                                                [ str "account_balance_wallet" ] ]
                                          div [ Class "content" ]
                                            [ div [ Class "card-title is-tile has-text-right" ]
                                                [ str "Sales"
                                                  div [ Class "card-stat primary has-text-right" ]
                                                    [ str "284"
                                                      span [ Class "stat-type" ]
                                                        [ str "this month" ] ] ] ]
                                          div [ Class "more" ]
                                            [ a [ Class "primary"
                                                  Href "#" ]
                                                [ str "See all" ] ] ] ]
                                  div [ Class "column is-3" ]
                                    [ div [ Class "flex-card card-overflow light-bordered light-raised" ]
                                        [ div [ Class "icon-header" ]
                                            [ i [ Class "material-icons primary" ]
                                                [ str "account_balance" ] ]
                                          div [ Class "content" ]
                                            [ div [ Class "card-title is-tile has-text-right" ]
                                                [ str "Account"
                                                  div [ Class "card-stat primary has-text-right" ]
                                                        [ span [ Class "stat-type" ]
                                                            [ str "$" ]
                                                          str "122 839,49" ] ] ]
                                          div [ Class "more" ]
                                            [ a [ Class "primary"
                                                  Href "#" ]
                                                [ str "Open" ] ] ] ]
                                  div [ Class "column is-3" ]
                                    [ div [ Class "flex-card card-overflow light-bordered light-raised" ]
                                        [ div [ Class "icon-header" ]
                                            [ i [ Class "material-icons secondary" ]
                                                [ str "work" ] ]
                                          div [ Class "content" ]
                                            [ div [ Class "card-title is-tile has-text-right" ]
                                                [ str "Deals"
                                                  div [ Class "card-stat secondary has-text-right" ]
                                                    [ str "14"
                                                      span [ Class "stat-type" ]
                                                        [ str "pending" ] ] ] ]
                                          div [ Class "more" ]
                                            [ a [ Class "secondary"
                                                  Href "#" ]
                                                [ str "See all" ] ] ] ]
                                  div [ Class "column is-3" ]
                                    [ div [ Class "flex-card card-overflow light-bordered light-raised" ]
                                        [ div [ Class "icon-header" ]
                                            [ i [ Class "material-icons secondary" ]
                                                [ str "business" ] ]
                                          div [ Class "content" ]
                                            [ div [ Class "card-title is-tile has-text-right" ]
                                                [ str "Leads"
                                                  div [ Class "card-stat secondary has-text-right" ]
                                                    [ str "53"
                                                      span [ Class "stat-type" ]
                                                        [ str "new leads" ] ] ] ]
                                          div [ Class "more" ]
                                            [ a [ Class "secondary"
                                                  Href "#" ]
                                                [ str "See all" ] ] ] ] ]
                              div [ Class "columns dashboard-columns" ]
                                [ div [ Class "column is-4" ]
                                    [ div [ Id "linechart-card"
                                            Class "flex-card light-bordered card-overflow light-raised" ]
                                        [ h3 [ Class "card-heading is-absolute" ]
                                            [ str "Sales Report" ]
                                          div [ Class "header-control ml-auto is-drop drop-sm" ]
                                            [ i [ Class "sl sl-icon-arrow-down" ]
                                                [ ]
                                              div [ Class "dropContain" ]
                                                [ div [ Class "dropOut" ]
                                                    [ ul [ ]
                                                        [ li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-refresh" ]
                                                                    [ ]
                                                                  str "Reload" ]
                                                          li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-printer" ]
                                                                    [ ]
                                                                  str "Print" ]
                                                          li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-settings" ]
                                                                    [ ]
                                                                  str "Settings" ] ] ] ] ]
                                          canvas [ Id "lineChart"]
                                                //    Width 400
                                                //    Height 300 ]
                                            [ ] ]
                                      div [ Class "flex-card is-squared light-bordered light-raised card-overflow has-text-centered" ]
                                        [ div [ Class "material-header is-secondary has-text-centered" ]
                                            [ h3 [ ]
                                                [ str "Sales income" ]
                                              span [ Class "widget-bars" ]
                                                [ str "5,3,9,6,5,9,7,3,5,16,5,7,2,8,6,4,8,2,5,6,8,9" ] ]
                                          div [ Class "card-body has-text-centered" ]
                                            [ div [ Class "stat-text" ]
                                                [ div [ Class "stat-average" ]
                                                    [ span [ ]
                                                        [ str "$" ]
                                                      span [ ]
                                                        [ str "118 595,49" ] ]
                                                  div [ Class "stat-subtitle" ]
                                                    [ str "Average sales per month" ] ] ] ]
                                      div [ Class "flex-card is-squared light-bordered light-raised padding-30 has-text-centered" ]
                                        [ svg [ Class "circle-chart"
                                                HTMLAttr.Custom ("viewbox", "0 0 33.83098862 33.83098862")
                                                // Width 140
                                                // Height 140
                                                HTMLAttr.Custom ("xmlns", "http://www.w3.org/2000/svg") ]
                                            [ circle [ Class "circle-chart-background"
                                                       HTMLAttr.Custom ("stroke", "#efefef")
                                                       HTMLAttr.Custom ("stroke-width", "2")
                                                       HTMLAttr.Custom ("fill", "none")
                                                       HTMLAttr.Custom ("cx", "16.91549431")
                                                       HTMLAttr.Custom ("cy", "16.91549431")
                                                       HTMLAttr.Custom ("r", "15.91549431") ]
                                                [ ]
                                              circle [ Class "circle-chart-circle"
                                                       HTMLAttr.Custom ("stroke", "#7F00FF")
                                                       HTMLAttr.Custom ("stroke-width", "2")
                                                       HTMLAttr.Custom ("stroke-dasharray", "68,100")
                                                       HTMLAttr.Custom ("stroke-linecap", "round")
                                                       HTMLAttr.Custom ("fill", "none")
                                                       HTMLAttr.Custom ("cx", "16.91549431")
                                                       HTMLAttr.Custom ("cy", "16.91549431")
                                                       HTMLAttr.Custom ("r", "15.91549431") ]
                                                [ ] ]
                                          div [ Class "chart-avatar" ]
                                            [ img [ Src "assets/images/avatars/helen.jpg"
                                                    Alt "" ] ]
                                          div [ Class "ring-title has-text-centered" ]
                                            [ span [ ]
                                                [ str "68% Deals Won" ] ] ] ]
                                  div [ Class "column is-4" ]
                                    [ div [ Id "vmap-container"
                                            Class "flex-card light-bordered card-overflow light-raised" ]
                                        [ div [ Id "vmap" ]
                                            [ ] ]
                                      div [ Class "flex-card is-squared light-bordered light-raised card-overflow has-text-centered" ]
                                        [ div [ Class "material-header is-primary has-text-centered" ]
                                            [ h3 [ ]
                                                [ str "Activity stream" ]
                                              span [ Class "widget-lines" ]
                                                [ str "5,3,9,6,5,9,7,3,5,16,5,7,2,8,6,4,8,2,5,6,8,9" ] ]
                                          div [ Class "card-body has-text-centered" ]
                                            [ div [ Class "stat-text" ]
                                                [ div [ Class "stat-average" ]
                                                    [ span [ ]
                                                        [ ]
                                                      span [ ]
                                                        [ str "543" ] ]
                                                  div [ Class "stat-subtitle" ]
                                                    [ str "Average comments per month" ] ] ] ]
                                      div [ Id "doughnut-card"
                                            Class "flex-card light-bordered card-overflow light-raised" ]
                                        [ h3 [ Class "card-heading is-absolute" ]
                                            [ str "Task progress" ]
                                          div [ Class "header-control ml-auto is-drop drop-sm" ]
                                            [ i [ Class "sl sl-icon-arrow-down" ]
                                                [ ]
                                              div [ Class "dropContain" ]
                                                [ div [ Class "dropOut" ]
                                                    [ ul [ ]
                                                        [ li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-refresh" ]
                                                                    [ ]
                                                                  str "Refresh" ]
                                                          li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-check" ]
                                                                    [ ]
                                                                  str "My tasks" ]
                                                          li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-rocket" ]
                                                                    [ ]
                                                                  str "All projects" ] ] ] ] ]
                                          canvas [ Id "doughnutChart"]
                                                //    Width 150
                                                //    Height 150 ]
                                            [ ]
                                          div [ Class "has-text-centered mt-50" ]
                                            [ a [ Class "button btn-dash secondary-btn btn-dash is-raised rounded ripple"
                                                  HTMLAttr.Custom ("data-ripple-color", "") ]
                                                [ str "See all tasks" ] ] ] ]
                                  div [ Class "column is-4" ]
                                    [ div [ Class "flex-card light-bordered light-raised padding-10" ]
                                        [ h3 [ Class "card-heading" ]
                                            [ str "Members" ]
                                          ul [ Class "user-list" ]
                                            [ li [ ]
                                                [ div [ Class "user-list-avatar" ]
                                                    [ img [ Src "assets/images/avatars/terry.jpg"
                                                            Alt "" ] ]
                                                  div [ Class "user-list-info" ]
                                                    [ div [ Class "name" ]
                                                        [ str "Terry Daniels" ]
                                                      div [ Class "position" ]
                                                        [ str "CEO" ] ]
                                                  div [ Class "user-list-status is-online" ]
                                                    [ ] ]
                                              li [ ]
                                                [ div [ Class "user-list-avatar" ]
                                                    [ img [ Src "assets/images/avatars/carolin.png"
                                                            Alt "" ] ]
                                                  div [ Class "user-list-info" ]
                                                    [ div [ Class "name" ]
                                                        [ str "Marjory Cambell" ]
                                                      div [ Class "position" ]
                                                        [ str "CFO" ] ]
                                                  div [ Class "user-list-status is-busy" ]
                                                    [ ] ]
                                              li [ ]
                                                [ div [ Class "user-list-avatar" ]
                                                    [ img [ Src "assets/images/avatars/ben.jpg"
                                                            Alt "" ] ]
                                                  div [ Class "user-list-info" ]
                                                    [ div [ Class "name" ]
                                                        [ str "Kevin Smith" ]
                                                      div [ Class "position" ]
                                                        [ str "Software Engineer" ] ]
                                                  div [ Class "user-list-status is-offline" ]
                                                    [ ] ]
                                              li [ ]
                                                [ div [ Class "user-list-avatar" ]
                                                    [ img [ Src "assets/images/avatars/melany.jpg"
                                                            Alt "" ] ]
                                                  div [ Class "user-list-info" ]
                                                    [ div [ Class "name" ]
                                                        [ str "Melany Wright" ]
                                                      div [ Class "position" ]
                                                        [ str "Sales Manager" ] ]
                                                  div [ Class "user-list-status is-online" ]
                                                    [ ] ]
                                              li [ ]
                                                [ div [ Class "user-list-avatar" ]
                                                    [ img [ Src "assets/images/avatars/helen.jpg"
                                                            Alt "" ] ]
                                                  div [ Class "user-list-info" ]
                                                    [ div [ Class "name" ]
                                                        [ str "Helen Miller" ]
                                                      div [ Class "position" ]
                                                        [ str "Sales Manager" ] ]
                                                  div [ Class "user-list-status is-online" ]
                                                    [ ] ] ] ]
                                      div [ Class "flex-card light-bordered light-raised padding-10" ]
                                        [ h3 [ Class "card-heading" ]
                                            [ str "Activity feed" ]
                                          div [ Class "header-control ml-auto is-drop drop-sm" ]
                                            [ i [ Class "sl sl-icon-arrow-down" ]
                                                [ ]
                                              div [ Class "dropContain" ]
                                                [ div [ Class "dropOut" ]
                                                    [ ul [ ]
                                                        [ li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-refresh" ]
                                                                    [ ]
                                                                  str "Refresh" ]
                                                          li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-user" ]
                                                                    [ ]
                                                                  str "My activity" ]
                                                          li [ ]
                                                                [ i [ Class "drop-icon sl sl-icon-people" ]
                                                                    [ ]
                                                                  str "All activity" ] ] ] ] ]
                                          ol [ Class "simple-feed" ]
                                            [ li [ Class "feed-item" ]
                                                [ div [ Class "feed-item-text" ]
                                                    [ span [ Class "date" ]
                                                        [ str "2 minutes ago" ]
                                                      span [ Class "text" ]
                                                            [ span [ Class "name" ]
                                                                [ str "Henry Rodstein" ]
                                                              str "sent you a Message." ] ] ]
                                              li [ Class "feed-item" ]
                                                [ div [ Class "feed-item-text" ]
                                                    [ span [ Class "date" ]
                                                        [ str "3 hours ago" ]
                                                      span [ Class "text" ]
                                                        [ str "You have 5 new messages." ] ] ]
                                              li [ Class "feed-item" ]
                                                [ div [ Class "feed-item-text" ]
                                                    [ span [ Class "date" ]
                                                        [ str "5 hours ago" ]
                                                      span [ Class "text" ]
                                                            [ span [ Class "name" ]
                                                                [ str "Marjory Cambell" ]
                                                              str "is now following you." ] ] ]
                                              li [ Class "feed-item" ]
                                                [ div [ Class "feed-item-text" ]
                                                    [ span [ Class "date" ]
                                                        [ str "Yesterday" ]
                                                      span [ Class "text" ]
                                                        [ str "You have 3  invoices pending." ] ] ]
                                              li [ Class "feed-item" ]
                                                [ div [ Class "feed-item-text" ]
                                                    [ span [ Class "date" ]
                                                        [ str "Monday" ]
                                                      span [ Class "text" ]
                                                            [ span [ Class "name" ]
                                                                [ str "Eleanor Briggs" ]
                                                              str "liked your Post." ] ] ]
                                              li [ Class "feed-item" ]
                                                [ div [ Class "feed-item-text" ]
                                                    [ span [ Class "date" ]
                                                        [ str "Last week" ]
                                                      span [ Class "text" ]
                                                            [ span [ Class "name" ]
                                                                [ str "Kevin Smith" ]
                                                              str "added a Project." ] ] ] ] ] ] ] ] ] ] ] ]
          div [ Class "column" ]
            [ ] ]