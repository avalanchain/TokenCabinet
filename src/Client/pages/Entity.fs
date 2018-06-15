module Client.Entity

open System
open FSharp.Reflection

open Fable.Core
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open Elmish

open ServerCode.Utils
open ServerCode.Result
open ServerCode.EntityList
open ServerCode.Domain
open ServerCode.Schema

open Style
open DomainView
open Client.Utils
open FormHelpers

/// The user data sent with every message.
type UserData = 
  { UserName : UserName 
    Token : JWT }

/// The different messages processed when interacting with the entity list
type EntityListMsg<'entity when 'entity: equality> =
  | LoadForUser of UserName
  | FetchedEntityList of FullSchemaDef * EntityList<'entity>
  | RemoveEntity of 'entity
  | AddEntity
  | FieldChanged of FieldName * string
  | FetchError of exn

type EntityModel<'entity when 'entity: equality> = {
    Fields: FieldDefList<'entity> 
    EntityList : EntityList<'entity>
    Schema: FullSchemaDef
    Token : JWT
    NewEntity: 'entity
    ErrorTexts : Map<FieldName, string list> 
    ErrorMsg : string 
}

type EntityDef<'entity, 'AppMsg when 'entity: equality> = {
    Fields: FieldDefList<'entity>
    Empty: unit -> 'entity
    Verifier: 'entity -> bool
    Sorter: 'entity -> string // TODO: Generalize sort key
    Title: string
    MessageCase: EntityListMsg<'entity> -> 'AppMsg
    ServerUrl: string
}
with
    member private __.DomainView = new DomainView<EntityList<'entity>, EntityListMsg<'entity>>(__.ServerUrl, FetchedEntityList, FetchError)
    [<PassGenerics>]
    member __.Init (user: UserData) = 
        {
            Fields = __.Fields
            EntityList = { UserName = user.UserName; Items = [] }
            Schema = { schema = createObj[]; uiSchema = createObj[]; schemaElement = SchemaElement.NOPE }
            Token = user.Token
            NewEntity = __.Empty()
            ErrorTexts = Map.empty
            ErrorMsg = ""
        }, __.DomainView.LoadAllCmd user.Token

    [<PassGenerics>]
    member __.Update (msg: EntityListMsg<'entity>) model : EntityModel<'entity> * Cmd<EntityListMsg<'entity>> = 
        Browser.console.info(sprintf "Update msg: %A" msg)
        match msg with
        | EntityListMsg.LoadForUser user ->
            model, []
        | FetchedEntityList (schema, entityList) ->
            let entityList = { entityList with Items = entityList.Items |> List.sortBy __.Sorter }
            Browser.console.info(sprintf "Update schema loaded: %A" schema)
            {   model with EntityList = entityList; Schema = {  schema = schema.schema.ToString() |> JS.JSON.parse
                                                                uiSchema = schema.uiSchema.ToString() |> JS.JSON.parse
                                                                schemaElement = schema.schemaElement } }, Cmd.none
        | FieldChanged (fieldName, value) -> 
            let lens = model.Fields |> List.filter (fun f -> f.FieldName = fieldName) |> List.head |> fun f -> f.Lens
            let validationResult = 
                let res = lens.ValidateValue value
                match res with
                | Ok _ -> []
                | Error errors -> errors
            { model with NewEntity = value |> (lens.Set model.NewEntity); ErrorTexts = model.ErrorTexts.Add(fieldName, validationResult) }, Cmd.none
        | RemoveEntity entity -> 
            let entityList = { model.EntityList with Items = model.EntityList.Items |> List.filter ((<>) entity) }
            { model with EntityList = entityList}, __.DomainView.PostCmd model.Token entityList 
        | AddEntity ->
            if __.Verifier model.NewEntity then
                let entityList = { model.EntityList with Items = (model.NewEntity :: model.EntityList.Items) |> List.sortBy __.Sorter }
                { model with EntityList = entityList; NewEntity = __.Empty() }, __.DomainView.PostCmd model.Token entityList
            else
                { model with ErrorTexts = Map.empty }, Cmd.none
        | FetchError e -> 
            Browser.console.info(sprintf "Update. Error occured: %A" e)
            model, Cmd.none

    [<PassGenerics>]
    member private __.EntityTableDef (dispatch: 'AppMsg -> unit) (model: EntityModel<'entity>): TableDef<'entity> = {
        Title = sprintf "%ss for %s" __.Title model.EntityList.UserName
        Columns = List.append 
                    (__.Fields |> List.map(fun f -> { Header = f.GetDisplayName |> splitOnCapital; Cell = fun book -> [ text (book |> f.Lens.GetString) ] }))
                    [{ Header = ""; Cell = fun entity -> [ buttonLink "" (fun _ -> dispatch (entity |> RemoveEntity |> __.MessageCase )) [ text "Remove" ] ] }]
    }

    [<PassGenerics>]
    member private __.NewEntityFormDef (model: EntityModel<'entity>) : NewFormDef<'entity, 'AppMsg> = 
        let getErrors fieldName = model.ErrorTexts |> Map.tryFind fieldName |> (fun elo -> if elo.IsNone then [] else elo.Value)
        let newField fieldName caption schemaElement value : FieldDef<'entity, 'AppMsg> =
            {   Name = caption
                IconClass =  "glyphicon-pencil"
                Placeholder =  "Please insert " + (splitOnCapital caption)
                ErrorText = getErrors (fieldName)
                Value = value
                DispatchMsg = fun msg -> EntityListMsg.FieldChanged (fieldName, msg) |> __.MessageCase 
                SchemaElement = schemaElement }
        {   Title = __.Title
            Fields = __.Fields |> List.map (fun f -> newField f.FieldName f.GetDisplayName f.SchemaElement f.Lens.GetString)
            Schema = model.Schema
            ButtonActiveCondition = __.Verifier
            SubmitMsg = EntityListMsg.AddEntity |> __.MessageCase  
        }

    [<PassGenerics>]
    member __.View (model: EntityModel<'entity>) (dispatch: 'AppMsg -> unit) = 
        FormHelpers.view dispatch (__.EntityTableDef dispatch model) model.EntityList.Items (__.NewEntityFormDef model) model.NewEntity


// let tableDef (dispatch: AppMsg -> unit) (model: Model<Book>): TableDef<Book> = {
//     Title = sprintf "Individuals for %s" model.EntityList.UserName
//     Columns = [
//                 { Header = "First Name"; Cell = fun book -> [if String.IsNullOrWhiteSpace book.Link then yield text book.Title
//                                                                 else yield a [ Href book.Link; Target "_blank"] [text book.Title ] ] }
//                 { Header = "Last Name"; Cell = fun book -> [ text book.Authors ] }
//                 { Header = "Email"; Cell = fun book -> [ text book.Note ] }
//                 { Header = ""; Cell = fun book -> [ buttonLink "" (fun _ -> dispatch (WishListMsg (RemoveEntity book))) [ text "Remove" ] ] }
//     ]
// }

// type (| StringField | IntField |) (propInfo: Reflection.PropertyInfo) = 
//     match 

[<PassGenerics>]
let generateFields<'entity when 'entity: equality> () =
    let generateRecordFields recordType = 
        let fields = recordType |> FSharpType.GetRecordFields
        let recordReader e = FSharpValue.GetRecordFields(e)
        let recordWriter vals = FSharpValue.MakeRecord(recordType, vals)

        let createFieldLens i (propInfo: Reflection.PropertyInfo) = 
            let propType = propInfo.PropertyType
            if propType = typedefof<float> then
                FloatField({get = fun e -> FSharpValue.GetRecordField (e, propInfo) :?> float
                            set = fun e v -> 
                                    let state = recordReader e
                                    state.[i] <- v :> obj
                                    recordWriter state :?> 'entity
                            validate = fun v -> Validation.verifyEmpty propInfo.Name (v.ToString()) })
            elif propType = typedefof<int> || propType = typedefof<int16> || propType = typedefof<uint16> || propType = typedefof<uint32> then
                IntField({  get = fun e -> FSharpValue.GetRecordField (e, propInfo) :?> int
                            set = fun e v -> 
                                    let state = recordReader e
                                    state.[i] <- v :> obj
                                    recordWriter state :?> 'entity
                            validate = fun v -> Validation.verifyEmpty propInfo.Name (v.ToString()) })
            else StringField({  get = fun e -> FSharpValue.GetRecordField (e, propInfo) :?> string
                                set = fun e v -> 
                                        let state = recordReader e
                                        state.[i] <- v :> obj
                                        recordWriter state :?> 'entity
                                validate = Validation.verifyEmpty propInfo.Name })

        let fieldDefs: FieldDefList<'entity> = 
            fields
            |> Array.mapi (fun i propInfo -> {  FieldName = FieldName(propInfo.Name)
                                                DisplayName = None
                                                Lens = createFieldLens i propInfo
                                                SchemaElement = SchemaElement.NOPE }) // TODO: Update with a proper element
            |> Array.toList
        fieldDefs
    
    let recordType = typeof<'entity>
    if FSharpType.IsRecord recordType then generateRecordFields recordType
    else []
    
