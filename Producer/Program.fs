// Learn more about F# at http://fsharp.org

open System.Text
open DotPulsar

[<EntryPoint>]
let main argv =
    let options = ProducerOptions("")

    let client =
        PulsarClient.Builder().Build().CreateProducer(options)

    let message = "message" |> Encoding.UTF8.GetBytes

    client.Send(message).AsTask()
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> fun m -> m.EntryId
    |> printfn "%d"
    |> ignore

    0 // return an integer exit code
