// Learn more about F# at http://fsharp.org

open System
open System.Text
open FSharp.Control
open DotPulsar
open Vulpecula

let printMessage (message: Message, id: int) =
    message.Data
    |> (fun d -> EncodingExtensions.GetString(Encoding.UTF8, &d))
    |> printfn "%d: %s" id

let acknowledge (client: Abstractions.IConsumer, message: Message) =
    async {
        do! client.Acknowledge(message).AsTask()
            |> Async.AwaitTask

        return message
    }

let receiveMessages (options: ConsumerOptions, id: int) =
    let client =
        PulsarClient.Builder().Build().CreateConsumer(options)

    printfn "Running %d" id

    client.Messages()
    |> AsyncSeq.ofAsyncEnum
    |> AsyncSeq.mapAsync (fun m -> acknowledge (client, m))
    |> AsyncSeq.map (fun m -> printMessage (m, id))
    |> ignore

    (id, client)

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

let rec waitTasks (tasks: Map<int, Abstractions.IConsumer>) =
    let id = Console.ReadLine() |> Convert.ToInt32

    tasks
    |> Map.tryFind id
    |> Option.iter (fun token ->
        printfn "Cancel %d" id
        token.DisposeAsync().AsTask()
        |> Async.AwaitTask
        |> Async.RunSynchronously
        waitTasks (tasks.Remove(id)))
    waitTasks (tasks)

[<EntryPoint>]
let main argv =
    argv
    |> Array.tryHead
    |> Option.map run
    |> Option.map Map.ofList
    |> Option.iter waitTasks

    0 // return an integer exit code
