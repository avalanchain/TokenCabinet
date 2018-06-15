module Client.DomainView

open System

open Fable.Core
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open Elmish

open ServerCode.Domain
open ServerCode.Schema
open Style

module RS = Fable.Import.ReactStrap

type DomainView<'data, 'msg> (relUrl, fetchMsg: FullSchemaDef * 'data -> 'msg, fetchError: Exception -> 'msg) =
/// Get the wish list from the server, used to populate the model
    [<PassGenerics>]
    member __.GetAll token: JS.Promise<FullSchemaDef * 'data> =
        promise {
            let url = "api/" + relUrl + "/"
            let schemaUrl = "api/" + relUrl + "/schema/"
            let props = 
                [ Fetch.requestHeaders [
                    HttpRequestHeaders.Authorization ("Bearer " + token) ]]

            let! ret = Fable.PowerPack.Fetch.fetchAs<'data> url props
            Browser.console.info(sprintf "Data loaded: %A" ret)
            let mutable sc = { schema = null; uiSchema = null; schemaElement = SchemaElement.NOPE }
            try 
                let! schema = Fable.PowerPack.Fetch.fetchAs<FullSchemaDef> schemaUrl props
                sc <- schema
            with 
                | e -> Browser.console.error(sprintf "Schema loading error: %A" e)
            return sc, ret
        }

    // [<PassGenerics>]
    // member __.GetFormSchema token: JS.Promise<string> =
    //     promise {        
    //         let url = "api/" + relUrl + "-schema/"
    //         let props = 
    //             [ Fetch.requestHeaders [
    //                 HttpRequestHeaders.Authorization ("Bearer " + token) ]]

    //         let! ret = Fable.PowerPack.Fetch.fetchAs<string> url props
    //         return ret
    //     }

    [<PassGenerics>]
    member __.LoadAllCmd (token: JWT) : Cmd<'msg> = Cmd.ofPromise (__.GetAll) token fetchMsg fetchError

    [<PassGenerics>]
    member __.Post (token, t): JS.Promise<FullSchemaDef * 'data> =
        promise {        
            let url = "api/" + relUrl
            let schemaUrl = "api/" + relUrl + "-schema"
            let body = toJson t
            let props = 
                [   RequestProperties.Method HttpMethod.POST
                    Fetch.requestHeaders [
                        HttpRequestHeaders.Authorization ("Bearer " + token)
                        HttpRequestHeaders.ContentType "application/json" ]
                    RequestProperties.Body (unbox body) ]

            let! ret = Fable.PowerPack.Fetch.fetchAs<'data> url props
            let! schema = Fable.PowerPack.Fetch.fetchAs<FullSchemaDef> schemaUrl props
            return schema, ret
        }

    [<PassGenerics>]
    member __.PostCmd token t : Cmd<'msg> = 
        Cmd.ofPromise (__.Post<'data>) (token, t) fetchMsg fetchError

module FormHelpers =
    let buttonActive condition = if condition then "btn-primary" else "btn-disabled"
    let toStatus value = if String.IsNullOrEmpty value then "" else "has-success"
    type FieldDef<'T, 'msg> = {
        Name: string
        IconClass: string
        Placeholder: string 
        ErrorText: string list
        Value: 'T -> string
        DispatchMsg: string -> 'msg
        SchemaElement: SchemaElement
    }

    [<PassGenerics>]
    let fieldDiv<'T, 'msg> (dispatch: 'msg -> unit) (model: 'T) (fieldDef: FieldDef<'T, 'msg>) =
        let value = model |> fieldDef.Value
        div [ClassName ("form-group has-feedback " + (toStatus value))] [
            yield div [ClassName "input-group"] [
                    yield span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + fieldDef.IconClass)] [] ]
                    yield input [
                            HTMLAttr.Type "text"
                            Name fieldDef.Name
                            Value (U2.Case1 value)
                            ClassName "form-control"
                            Placeholder fieldDef.Placeholder
                            Required true
                            OnChange (fun (ev:React.FormEvent) -> (unbox ev.target?value)
                                                                    |> fieldDef.DispatchMsg
                                                                    |> dispatch) ]
                    match fieldDef.ErrorText with
                    | e :: _ -> yield span [ClassName "glyphicon glyphicon-remove form-control-feedback"] []
                    | _ -> ()
            ]
            match fieldDef.ErrorText with
            | e :: _ -> yield p [ClassName "text-danger"][text e]
            | _ -> ()
        ]

    [<PassGenerics>]
    let fieldUnionDiv<'T, 'msg> (dispatch: 'msg -> unit) (model: 'T) (fieldDef: FieldDef<'T, 'msg>) =
        let value = model |> fieldDef.Value
        div [ClassName ("form-group has-feedback " + (toStatus value))] [
            div [ClassName "input-group"] [
                    yield span [ClassName "input-group-addon"] [span [ClassName ("glyphicon " + fieldDef.IconClass)] [] ]
                    yield input [
                            HTMLAttr.Type "text"
                            Name fieldDef.Name
                            Value (U2.Case1 value)
                            ClassName "form-control"
                            Placeholder fieldDef.Placeholder
                            Required true
                            OnChange (fun (ev:React.FormEvent) -> (unbox ev.target?value)
                                                                    |> fieldDef.DispatchMsg
                                                                    |> dispatch) ]
                    match fieldDef.ErrorText with
                    | e :: _ -> yield span [ClassName "glyphicon glyphicon-remove form-control-feedback"] []
                    | _ -> ()
            ]
            // match fieldDef.ErrorText with
            // | e :: _ -> yield p [ClassName "text-danger"][text e]
            // | _ -> ()
        ]

    type NewFormDef<'T, 'msg> = {
        Title: string
        Fields: FieldDef<'T, 'msg> list
        Schema: FullSchemaDef
        ButtonActiveCondition: 'T -> bool
        SubmitMsg: 'msg
    }

    [<Pojo>]
    type FieldProps = {
        schema: obj
        uiSchema: obj 
        idSchema: obj
        formData: obj
        errorSchema: obj
        registry: obj
        formContext: obj
    }

    let EnumElement (props: FieldProps): React.ReactElement = div [] [text title]

    [<Pojo>]
    type FormDef = {
        schema: obj
        uiSchema: obj 
        widgets: obj
        liveValidate: bool
        showErrorList: bool
    }

    //let [<Import("Form","react-jsonschema-form")>] Form: SchemaDefs -> React.ReactElement = jsNative
    let Form: FormDef -> React.ReactElement = importDefault "react-jsonschema-form"

    [<PassGenerics>]
    let newItemForm<'T, 'msg> (dispatch: 'msg -> unit) (def: NewFormDef<'T, 'msg>) (t: 'T) =
        Browser.console.info(sprintf "Form schema loaded: %A" def.Schema.schema)
        Browser.console.info(sprintf "Form UI schema loaded: %A" def.Schema.uiSchema)
        div [] [
            h4 [] [text title]
            fn Form {   schema = def.Schema.schema
                        uiSchema = def.Schema.uiSchema
                        liveValidate = true
                        showErrorList = false
                        widgets = createObj [ "uiCustomDU", EnumElement :> obj ] } []
            h4 [] [text title]

            ///// Old Add Form
            div [ClassName "container"] [
                div [ClassName "row"] [
                    div [ClassName "col-md-8"] (
                        (def.Fields |> List.map (fieldDiv dispatch t))
                            @ [
                                button [ ClassName ("btn " + (buttonActive (def.ButtonActiveCondition t))); OnClick (fun _ -> def.SubmitMsg |> dispatch)] [
                                    i [ClassName "glyphicon glyphicon-plus"; Style [PaddingRight 5]] []
                                    text "Add"
                                ]
                            ])                    
                ]        
            ]
        ]

    type TableColumnDef<'T> = {
        Header: string
        Cell: 'T -> React.ReactElement list
    }

    type TableDef<'T> = {
        Title: string
        Columns: TableColumnDef<'T> list
    }

    [<PassGenerics>]
    let listTable<'T> (def: TableDef<'T>) (models: 'T list) = 
        div [] [
            h4 [] [ text def.Title ]
            table [ClassName "table table-striped table-hover"] [
                thead [] [
                        tr [] (def.Columns |> List.map (fun c -> th [] [ text c.Header ]))
                ]                
                tbody[] [
                    for model in models do
                        yield tr [] [
                            for column in def.Columns do yield td [] (column.Cell model) 
                        ]
                ]
            ]
        ]

    

    [<PassGenerics>]
    let view<'T, 'msg> (dispatch: 'msg -> unit) (tableDef: TableDef<'T>) (tableModels: 'T list) (newFormDef: NewFormDef<'T, 'msg>) (newModel: 'T) = 
        div [] [
            listTable tableDef tableModels 
            
            newItemForm dispatch newFormDef newModel
        ]