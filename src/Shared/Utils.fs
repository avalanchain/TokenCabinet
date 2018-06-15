module ServerCode.Utils

open System.Text.RegularExpressions
open FSharp.Reflection
open Fable.Core

let join (p: Map<'a,'b>) (q: Map<'a,'b>) = Map(Seq.concat [ (Map.toSeq p) ; (Map.toSeq q) ])

let splitOnCapital caption = Regex.Replace(caption, "([a-z])([A-Z])", "$1 $2")

let [<PassGenerics>] getUnionCase (x:'a) = FSharpValue.GetUnionFields(x, typeof<'a>) |> fst

let [<PassGenerics>] getUnionCases<'t> = 
    FSharpType.GetUnionCases typeof<'t>
    |> Seq.filter(fun c -> c.Name <> "Book")
    |> List.ofSeq

let [<PassGenerics>] allUnionCases<'T>() =
    FSharpType.GetUnionCases(typeof<'T>)
    |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> 'T)
    |> Array.toList

let [<PassGenerics>] getUnionCaseName (x:'a) = 
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name  

let [<PassGenerics>] getUnionCaseNameSplit x = x |> getUnionCaseName |> splitOnCapital

///Returns the case names of union type 'ty.
let [<PassGenerics>] getUnionCaseNames<'ty> = 
    FSharpType.GetUnionCases(typeof<'ty>) |> Array.map (fun info -> info.Name) |> Array.toList

let [<PassGenerics>] getUnionCaseNamesSplit<'ty> = getUnionCaseNames<'ty> |> (List.map splitOnCapital)