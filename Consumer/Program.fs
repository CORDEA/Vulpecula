// Learn more about F# at http://fsharp.org

open FSharp.Control
open DotPulsar

let printMessage (message: Message) = message.Data.ToString() |> printfn "%s"

[<EntryPoint>]
let main argv =
    let options = ConsumerOptions("", "")

    let client =
        PulsarClient.Builder().Build().CreateConsumer(options)

    client.Messages()
    |> AsyncSeq.ofAsyncEnum
    |> AsyncSeq.map printMessage
    |> AsyncSeq.toArraySynchronously
    |> ignore

    0 // return an integer exit code
