module MyMarketBot.Program

open System
open MyMarketBot.Telegram
open MyMarketBot.Moex.Index

let asyncWait async = (Async.StartAsTask async).Wait()

[<EntryPoint>]
let main _ =
    let token = Environment.GetEnvironmentVariable "TELEGRAM_BOT_TOKEN"
    let chatId = Environment.GetEnvironmentVariable "TELEGRAM_SUBSCRIBER_ID" |> Int64.Parse
   
    use bot = run token
    
    async {
        let! indexes =
             moexIndexes
             |> Array.map (fun index -> (loadData index (DateTime.Now)))
             |> Async.Parallel
        let message = Message.prepareMessage indexes
        do! send chatId message bot |> Async.Ignore
    } |> asyncWait
    
    #if DEBUG
    Console.WriteLine("Press any key to continue...")
    Console.ReadKey() |> ignore
    #endif
    
    0
