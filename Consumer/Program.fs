// Learn more about F# at http://fsharp.org

open System.Text
open FSharp.Control
open DotPulsar
open Vulpecula

let printMessage (message: Message, id: int) =
    message.Data
    |> (fun d -> EncodingExtensions.GetString(Encoding.UTF8, &d))
    |> printfn "%d: %s" id

let receiveMessages (options: ConsumerOptions, id: int) =
    let client =
        PulsarClient.Builder().Build().CreateConsumer(options)

    printfn "Running %d" id
    client.Messages()
    |> AsyncSeq.ofAsyncEnum
    |> AsyncSeq.map (fun m -> printMessage (m, id))

let run (mode: string) =
    let mode =
        match mode with
        | "exclusive" -> SubscriptionType.Exclusive
        | "failover" -> SubscriptionType.Failover
        | "shared" -> SubscriptionType.Shared

    let options =
        ConsumerOptions(Constants.SubscriptionName, Constants.Topic)

    options.SubscriptionType <- mode

    List.init 5 (fun i -> i)
    |> List.map (fun i -> receiveMessages (options, i))
    |> AsyncSeq.mergeAll
    |> AsyncSeq.toArraySynchronously
    |> ignore

[<EntryPoint>]
let main argv =
    argv |> Array.tryHead |> Option.iter run |> ignore
    0 // return an integer exit code
