// Learn more about F# at http://fsharp.org

open System.Text
open FSharp.Control
open DotPulsar
open Vulpecula

let printMessage (message: Message) =
    message.Data
    |> (fun d -> EncodingExtensions.GetString(Encoding.UTF8, &d))
    |> printfn "%s"

[<EntryPoint>]
let main argv =
    let options =
        ConsumerOptions(Constants.SubscriptionName, Constants.Topic)

    let client =
        PulsarClient.Builder().Build().CreateConsumer(options)

    client.Messages()
    |> AsyncSeq.ofAsyncEnum
    |> AsyncSeq.map printMessage
    |> AsyncSeq.toArraySynchronously
    |> ignore

    0 // return an integer exit code
