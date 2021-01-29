module MyMarketBot.Program

open System
open MyMarketBot.Telegram
open MyMarketBot.Moex

let sendIndexData chatId bot index = async {
    let! data = loadData index DateTime.Today
    do! send chatId (sprintf "%s: %A" index data) bot |> Async.Ignore
}

[<EntryPoint>]
let main _ =
    let token = Environment.GetEnvironmentVariable "TELEGRAM_BOT_TOKEN"
    let chatId = Environment.GetEnvironmentVariable "TELEGRAM_SUBSCRIBER_ID" |> Int64.Parse
   
    use bot = run token
    
    let task = 
        [| "IMOEX"; "RTSI"; "RGBITR" |]
        |> Array.map (sendIndexData chatId bot)
        |> Async.Parallel
        |> Async.StartAsTask
        
    task.Wait()
    0
