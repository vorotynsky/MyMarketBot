module MyMarketBot.Program

open System
open MyMarketBot.Telegram
open MyMarketBot.Moex
open MyMarketBot.Moex.Index

let asyncWait async = (Async.StartAsTask async).Wait()

let load f = Array.map (fun ticker -> f ticker (DateTime(2021, 1, 27))) >> Async.Parallel
    

[<EntryPoint>]
let main _ =
    let token = Environment.GetEnvironmentVariable "TELEGRAM_BOT_TOKEN"
    let chatId = Environment.GetEnvironmentVariable "TELEGRAM_SUBSCRIBER_ID" |> Int64.Parse
   
    use bot = run token
    
    async {
        let! indexes = moexIndexes |> load Index.loadData
        let! currencies = [| "USD"; "EUR" |] |> load Currency.loadData
        
        let message = Message.prepareMessage indexes currencies
        do! send chatId message bot |> Async.Ignore
    } |> asyncWait
    
    #if DEBUG
    Console.WriteLine("Press any key to continue...")
    Console.ReadKey() |> ignore
    #endif
    
    0
