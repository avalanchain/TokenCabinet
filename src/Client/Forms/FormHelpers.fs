namespace Client.Forms

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Client.Style
open ServerCode.Utils
open ServerCode.Commodities

open ReactStrap
open ReactJsonSchemaForm
open ReactDatePicker
open Fable.Import.Moment
open Fable.Helpers.Moment

module RSel = Fable.Import.ReactSelect
module O = FSharp.Core.Option


module FormHelpers =
    open CustomControls
    let buttonActive condition = if condition then "btn-primary" else "btn-disabled"
    let toStatus value = if String.IsNullOrEmpty value then "" else "has-success"
    let isEmptyError fieldCaption value = if String.IsNullOrEmpty value then [ "Please enter " + fieldCaption ] else []
    let toNumberStatus value = if value <= 0. then "" else "has-success"
    let isEmptyNumberError fieldCaption value = if value <= 0. then [ "Please enter " + fieldCaption ] else []
    let toPriceStatus (value: Price) = toNumberStatus (value |> float)
    let isEmptyPriceError fieldCaption (value: Price) = isEmptyNumberError fieldCaption (value |> float)

    type FieldProps<'T> = {
        value: 'T
        onChange: 'T -> unit
        fieldName: string
        required: bool 
        title: string option
        iconClass: string
        placeholder: string option
        fieldStatus: 'T -> string
        validators: string -> 'T -> string list
    }

    let [<PassGenerics>] textFieldInner (innerProps: IHTMLProp list) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            input ([HTMLAttr.Type "text"
                    Name fieldName
                    Value (U2.Case1 value)
                    ClassName "form-control"
                    Placeholder placeholder 
                    Required required
                    OnChange (fun (ev:React.FormEvent) -> (unbox ev.target?value) |> onChange) ] @ innerProps)
        ]
    let [<PassGenerics>] numberFieldInner (innerProps: IHTMLProp list) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            input ([HTMLAttr.Type "number"
                    Name fieldName
                    Value (!!value)
                    ClassName "form-control"
                    Placeholder placeholder 
                    Required required
                    OnChange (fun (ev:React.FormEvent) -> (unbox ev.target?value) |> onChange) ] @ innerProps) 
        ] 

    let [<PassGenerics>] enumFieldInner<'T> (innerProps: IHTMLProp list) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            enumControl<'T> value onChange  
        ] 

    let [<PassGenerics>] volumeFieldInner (innerProps: IHTMLProp list) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            volumeControl value onChange  
        ] 

    let [<PassGenerics>] organizationFieldInner (organizations: Organization[]) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            organizationControl value onChange organizations
        ]

    let [<PassGenerics>] qualitySpecsFieldInner (innerProps: IHTMLProp list) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            qualitySpecsControl value onChange 
        ]
        
    let [<PassGenerics>] datePickerFieldInner (innerProps: IHTMLProp list) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            datePickerControl value onChange 
        ]

    let [<PassGenerics>] fileUploadControlInner (innerProps: IHTMLProp list) fieldName iconClass placeholder required onChange value = 
        [   //span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + iconClass)] [] ]
            fileUploadControl value onChange 
        ]

    let [<PassGenerics>] formField<'T, 'extra> (extra: 'extra) fieldInner (props: FieldProps<'T>) =
        let caption = match props.title with Some t -> t | None -> (props.fieldName |> splitOnCapital)
        let errors = (props.value |> props.validators caption)
        let placeholder = match props.placeholder with Some t -> t | None -> "Please enter " + caption
        div [ClassName ("form-group has-feedback " + (props.value |> props.fieldStatus))] [
            yield label [] [ text (caption + ":" (*+ if props.required then "*" else ""*) ) ]
            yield div [(*ClassName "input-group"*)] [
                yield! fieldInner extra props.fieldName props.iconClass placeholder props.required props.onChange props.value 
                if not errors.IsEmpty then yield span [ClassName "glyphicon glyphicon-remove form-control-feedback"] []
            ]
            for e in errors do yield p [ClassName "text-danger"] [text e]
        ]

    let [<PassGenerics>] textField = formField [] textFieldInner
    let [<PassGenerics>] numberField<'f> stepSize = formField<'f, IHTMLProp list> [ Step(U2.Case1 stepSize) ] numberFieldInner
    let [<PassGenerics>] enumField<'T> = formField [] enumFieldInner<'T>
    let [<PassGenerics>] volumeField = formField [] volumeFieldInner
    let [<PassGenerics>] organizationField organizations = formField organizations organizationFieldInner
    let [<PassGenerics>] qualitySpecsField = formField [ ] qualitySpecsFieldInner
    let [<PassGenerics>] datePickerField = formField [ ] datePickerFieldInner
    let [<PassGenerics>] fileUploadField = formField [ ] fileUploadControlInner