namespace ServerCode.EntityList

open System
open ServerCode.Result
open ServerCode.Domain

open Fable.Core

open ServerCode.Utils
open ServerCode.Schema


type Login = 
    { UserName : UserName
      Password : string }

type FieldName = FieldName of string
with member __.Name = match __ with FieldName s -> s

type EntityList<'entity when 'entity: equality> = {
    UserName: UserName
    Items: 'entity list
}
with
    static member New userName = 
        { UserName = userName
          Items = [] }

[<ReferenceEquality>]
type TypedEntityFieldLens<'entity, 'a> = {
    get: 'entity -> 'a
    set: 'entity -> 'a -> 'entity
    validate: 'a -> Result<unit, string list>
} 

type EntityFieldLens<'entity when 'entity: equality> = 
| StringField of TypedEntityFieldLens<'entity, string>
| IntField of TypedEntityFieldLens<'entity, int>
| BoolField of TypedEntityFieldLens<'entity, bool>
| FloatField of TypedEntityFieldLens<'entity, float>
with 
    [<PassGenerics>]
    member __.Get e = match __ with 
                        | StringField l -> l.get e :> obj
                        | IntField l -> l.get e :> obj
                        | BoolField l -> l.get e :> obj
                        | FloatField l -> l.get e :> obj
    [<PassGenerics>]
    member __.GetString e = match __ with 
                            | StringField l -> l.get e
                            | IntField l -> l.get e |> string
                            | BoolField l -> l.get e |> string
                            | FloatField l -> l.get e |> string
    [<PassGenerics>]
    member __.Set e (v: obj) = match __ with 
                                | StringField l -> l.set e (v :?> string)
                                | IntField l -> l.set e (v :?> int)
                                | BoolField l -> l.set e (v :?> bool)
                                | FloatField l -> l.set e (v :?> float)
    [<PassGenerics>]
    member __.ValidateValue (v: obj) = match __ with 
                                        | StringField l -> (v :?> string) |> l.validate 
                                        | IntField l -> (v :?> int) |> l.validate
                                        | BoolField l -> (v :?> bool) |> l.validate
                                        | FloatField l -> (v :?> float) |> l.validate
    [<PassGenerics>]
    member __.Validate e = match __ with 
                                    | StringField l -> e |> l.get |> l.validate 
                                    | IntField l -> e |> l.get |> l.validate
                                    | BoolField l -> e |> l.get |> l.validate
                                    | FloatField l -> e |> l.get |> l.validate

type FieldDef<'entity when 'entity: equality> = {
    FieldName: FieldName 
    DisplayName: string option
    Lens: EntityFieldLens<'entity>
    SchemaElement: SchemaElement
}
with member __.GetDisplayName = if __.DisplayName.IsSome then __.DisplayName.Value else __.FieldName.Name

type FieldDefList<'entity when 'entity: equality> = FieldDef<'entity> list

module Validation =
    let isOk x = match x with | Ok _ -> true | Error _ -> false
    let isError x = isOk >> not

    [<PassGenerics>]
    let verifyEmpty fieldName v =
        if String.IsNullOrWhiteSpace v then Error [sprintf "No %s was entered" fieldName |> splitOnCapital] else Ok()      

    [<PassGenerics>]
    let verifyEntity (fields: FieldDefList<_>) entity =
        fields |> Seq.forall(fun l -> l.Lens.Validate entity |> isOk)

    [<PassGenerics>]
    let verifyEntityList (fields: FieldDefList<_>) entityList =
        entityList.Items |> List.forall (verifyEntity fields)
        

/// The data for each book in /api/book
type Book = 
    { Title: string
      Authors: string
      Link: string 
      Note: string }

    static member empty = 
        { Title = ""
          Authors = ""
          Link = "" 
          Note = "" }
    static member Fields: FieldDefList<Book> = 
        [   {   FieldName = FieldName("Title")
                DisplayName = Some "First Name"
                Lens = StringField({get = fun e -> e.Title
                                    set = fun e v -> { e with Title = v }
                                    validate = Validation.verifyEmpty "title" })
                SchemaElement = SchemaElement.String }
            {   FieldName = FieldName("Authors")
                DisplayName = Some "Last Name"
                Lens = StringField({get = fun e -> e.Authors
                                    set = fun e v -> { e with Authors = v }
                                    validate = Validation.verifyEmpty "authors" })
                SchemaElement = SchemaElement.String }
            {   FieldName = FieldName("Link")
                DisplayName = Some "Middle Name"
                Lens = StringField({get = fun e -> e.Link
                                    set = fun e v -> { e with Link = v }
                                    validate = Validation.verifyEmpty "link" })
                SchemaElement = SchemaElement.String }
            {   FieldName = FieldName("Note")
                DisplayName = Some "Email"
                Lens = StringField({get = fun e -> e.Note
                                    set = fun e v -> { e with Note = v }
                                    validate = Validation.verifyEmpty "note" })
                SchemaElement = SchemaElement.String }
            ] 
    static member VerifyBook book = Validation.verifyEntity (Book.Fields) book


