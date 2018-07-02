module Client.FormHelpers

open Fable.Helpers.React
open Fable.Helpers.React.Props
// open ClientMsgs
open Fable
open Shared.ViewModels
open Fable.DateFunctions
open System
open Fable.Import.React
open Fable.Core.JsInterop
open ReactBootstrap
open Client.Helpers
open ReactBootstrap.Radio
open ReactBootstrap.Checkbox
open System.Collections.Generic

type FormElement =
    | Input of InputType
    | Select of (string * string) list 
    | Radio
    | Checkbox
    | Textarea
    | Static
and InputType =
    | Email
    | Text
    | Date
    | Submit
    | Number
    | File


let inputType inType =
    (match inType with 
                    | Email     -> "email"
                    | Text      -> "text"
                    | Date      -> "date"
                    | Submit    -> "submit"
                    | Number    -> "number"
                    | File      -> "file")

let inputControl inType = comF formControl (fun o -> o.``type`` <- Some (inputType inType))[]

let selectControl optionList = comF formControl (fun o ->  o.componentClass <- Some "select")
                                [ for name, value in optionList ->
                                    option [ Value name ][ str value ]
                                    // comF option (fun o ->  o.componentClass <- Some )
                                ]
let fGroupI element labelText helpText = 
    
         [
           comF controlLabel  (fun o -> o.className <- Some "col-sm-2" )
            [ str labelText ]

           div[ Class "col-sm-10"]
              [
                
                (
                match element with 
                | Input inputType       -> inputControl inputType
                | Select optionList     -> selectControl optionList
                | Radio                 -> inputControl InputType.Text
                | Checkbox              -> inputControl InputType.Text
                | Textarea              -> inputControl InputType.Text
                | Static                -> inputControl InputType.Text
                )
                span [ Class "help-block m-b-none text-muted" ]
                     [ str helpText ]
              ]
           ]
let fGroupO (element:FormElement) labelText helpText = 
    comE formGroup (fGroupI element labelText helpText)