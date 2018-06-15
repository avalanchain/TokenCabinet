namespace ServerCode.Domain

open System

open Fable.Core

open ServerCode.Schema

type JWT = string
type UserName = string

// Login credentials.
type Login = 
    { UserName : string
      Password : string }

[<Pojo>]
type FullSchemaDef = {
    schema: obj
    uiSchema: obj
    schemaElement: SchemaElement
}

type IdType = Guid
type DRef<'T> = { Id: IdType }
type DEntry<'T> = { Id: IdType; Entry: 'T }
type Ref<'T> = 
    | Ref of DRef<'T>
    | Entry of DEntry<'T>
    with 
        member __.Id = match __ with | Ref r -> r.Id | Entry e -> e.Id 
        [<PassGenerics>]
        static member New (entry: 'T) = { Id = Guid.NewGuid(); Entry = entry } |> Entry

type RefList<'T> = Ref<'T> list
type 'T dref = Ref<'T>
type 'T dreflist = RefList<'T>
