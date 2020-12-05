// Learn more about F# at http://fsharp.org

open System.Text
open FSharp.Control
open DotPulsar
open Vulpecula

let printMessage (message: Message) =
    message.Data
    |> (fun d -> EncodingExtensions.GetString(Encoding.UTF8, &d))
    |> printfn "%s"

let run (mode: string) =
    let mode =
        match mode with
        | "exclusive" -> SubscriptionType.Exclusive
        | "failover" -> SubscriptionType.Failover
        | "shared" -> SubscriptionType.Shared

    let options =
        ConsumerOptions(Constants.SubscriptionName, Constants.Topic)

    options.SubscriptionType <- mode

    let client =
        PulsarClient.Builder().Build().CreateConsumer(options)

    client.Messages()
    |> AsyncSeq.ofAsyncEnum
    |> AsyncSeq.map printMessage
    |> AsyncSeq.toArraySynchronously
    |> ignore

[<EntryPoint>]
let main argv =
    argv |> Array.tryHead |> Option.iter run |> ignore
    0 // return an integer exit code
