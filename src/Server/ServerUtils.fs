module ServerUtils

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.ExceptionServices
open FSharp.Control.Tasks
open System.Threading.Tasks

module Result =

    let inline isOk r = match r with | Ok _ -> true | Error _ -> false
    let inline isError r = r |> isOk |> not

    let reWrap (r: Result<Task<'T>, 'E>) : Task<Result<'T, 'E>> = task {
        return! 
            match r with
            | Ok okTask -> task {   let! res = okTask
                                    return Ok res  }  
            | Error e -> e |> Error |> Task.FromResult
    }


module TaskResult =
    open System.Threading.Tasks
    open FSharp.Control.Tasks.V2.ContextInsensitive

    let inline ofOption error = function Some s -> Ok s | None -> Error error

    module Task =
        let inline map f (t: Task<_>) = task {  let! v = t 
                                                return f v }

    let inline toOk v = Task.map Ok v

    type TaskResultBuilder() =
        member __.Return(x): Task<Result<_,_>> = x |> Ok |> Task.FromResult

        // member __.ReturnFrom(m: Task<_>) = task {   let! v = m 
        //                                             return Ok v }
        member __.ReturnFrom(m: Result<_, _>) = m |> Task.FromResult
        member __.ReturnFrom(m: Task<Result<_, _>>) = m 


        member inline __.bind(m: Task<Result<'T, 'E>>, f: 'T -> Task<Result<'U, 'E>>) = task {  let! v = m
                                                                                                return! match v with
                                                                                                            | Ok vr -> f vr
                                                                                                            | Error e -> Error e |> Task.FromResult }

        member __.Bind(m: Task<Result<'T, 'E>>, f: 'T -> Task<Result<'U, 'E>>) = __.bind(m, f)
        // member __.Bind(m: Task<'T>, f: 'T -> Task<Result<'U, 'E>>) = task { let! v = m
        //                                                                     return f v }
        member __.Bind(m: Result<'T, 'E>, f: 'T -> Task<Result<'U, 'E>>) = task {   return! match m with
                                                                                            | Ok vr -> f vr
                                                                                            | Error e -> Error e |> Task.FromResult }

        /// Binding to (Error, Option<'T>) tuple in order to make interop with functions returning Option<'T> easier
        /// Having error as the first parameter is surprisingly more natural in usage
        member __.Bind((error: 'E, m: Option<'T>), (f: 'T -> Task<Result<'U, 'E>>)) = __.Bind(m |> ofOption error, f)

        member __.Bind((errorMapper: 'E1 -> 'E2, m: Task<Result<'T, 'E1>>), f: 'T -> Task<Result<'U, 'E2>>) = 
            task {  let! v = m
                    return! match v with
                                | Ok vr -> f vr
                                | Error e -> e |> errorMapper |> Error|> Task.FromResult }

        member __.Combine(m, f) = __.bind(m, f)

        member __.Delay(f: unit -> _) = f

        member __.Run(f) = f()

        member __.TryWith(m: Task<Result<'T, 'E>>, h): Task<Result<'T, 'E>> =
            try m
            with e -> h e |> Task.FromResult

        member __.TryFinally(m: Task<Result<'T, 'E>>, compensation) =
            try m
            finally compensation()

        member __.Using(res:#System.IDisposable, body) =
            __.TryFinally(body res, fun () -> match res with null -> () | disp -> disp.Dispose())

        member __.Zero : Task<Result<unit, _>> = Ok () |> Task.FromResult

        // member __.While(guard, f) = task {
        //     let! g = guard() 
        //     let mutable g1 = g
        //     while g1 do
        //         f() |> ignore
        //         let! g = guard()
        //         g1 <- g
        // }

        // member __.For(sequence:seq<_>, body) =
        //     __.Using(sequence.GetEnumerator(), fun enum -> __.While(enum.MoveNext, __.Delay(fun () -> body enum.Current)))

    let taskResult = new TaskResultBuilder()

    let private a: Task<Result<int, string>> = 
        taskResult {
            let! tr1 = task { return Ok 3 }
            let! r1 = Ok tr1
            let! t1 = task { return r1 } |> Task.map Ok
            let! t2 = task { return r1.ToString() } |> toOk
            let! o1 = "error", Some t1
            let! tr2 = (fun ce -> ce.ToString()), task { return Error 'c' }
            let! tr3 = (fun ce -> ce.ToString()), task { return Ok "'c'" }
            // return t1
            return! Ok tr2
        }



    // type TaskResultBuilder2 () =
    //     member __.Return value : Step<Result<'T, 'Error>> =
    //         Ok value
    //         |> task.Return

    //     member __.ReturnFrom (taskResult : Step<Result<'T, 'Error>>) =
    //         taskResult

    //     member inline this.Zero () : Task<Result<unit, 'Error>> =
    //         task { return Ok () } 

    //     member inline this.Delay (generator : unit -> Step<Result<'T, 'Error>>) =
    //         task.Delay generator

    //     member __.Combine (r1: Task<Result<'T, 'Error>>, r2: Task<Result<'T, 'Error>>) : Task<Result<'T, 'Error>> =
    //         task {
    //             let! r1' = r1
    //             match r1' with
    //             | Error error ->
    //                 return Error error
    //             | Ok _ ->
    //                 return! r2
    //         }

    //     member __.Bind (value : Task<Result<'T, 'Error>>, binder : 'T -> Task<Result<'U, 'Error>>)
    //         : Task<Result<'U, 'Error>> =
    //         task {
    //             let! value' = value
    //             match value' with
    //             | Error error ->
    //                 return Error error
    //             | Ok x ->
    //                 return! binder x
    //         }

    //     member inline __.TryWith (computation : unit -> Step<Result<'T, 'Error>>, catchHandler : exn -> Step<Result<'T, 'Error>>) =
    //         task.TryWith(computation, catchHandler)

    //     member inline __.TryFinally (computation : unit -> Step<Result<'T, 'Error>>, compensation : unit -> unit) =
    //         task.TryFinally (computation, compensation)

    //     member inline __.Using (resource : ('T :> System.IDisposable), binder : _ -> Step<Result<'U, 'Error>>) =
    //         task.Using (resource, binder)

    //     member this.While (guard, body : Task<Result<unit, 'Error>>) =
    //         if guard () then
    //             this.Bind (body, (fun () -> this.While (guard, body)))
    //         else
    //             this.Zero ()

    //     // member this.For (sequence : seq<_>, body : 'T -> Task<Result<unit, 'Error>>) =
    //     //     this.Using (sequence.GetEnumerator (), fun enum ->
    //     //         this.While (
    //     //             enum.MoveNext,
    //     //             this.Delay (fun () ->
    //     //                 body enum.Current)))


    // let taskResult2 = TaskResultBuilder2()
    type TaskResult<'a> = Task<Result<'a, exn>>

    