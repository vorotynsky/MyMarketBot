module MyMarketBot.Program

open System
open MyMarketBot.Telegram

[<EntryPoint>]
let main _ =
    let token = Environment.GetEnvironmentVariable "TELEGRAM_BOT_TOKEN"
    
    use bot = run token
    
    Console.WriteLine "Press key to stop"
    Console.ReadKey() |> ignore
    0
