// Learn more about F# at http://fsharp.org

open DotPulsar

[<EntryPoint>]
let main argv =
    let options = ProducerOptions("")

    let client =
        PulsarClient.Builder().Build().CreateProducer(options)

    client.Send([||]).AsTask()
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> fun m -> m.EntryId
    |> printfn "%d"
    |> ignore

    0 // return an integer exit code
