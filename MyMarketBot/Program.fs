module MyMarketBot.Program

open System
open System.Text
open MyMarketBot.Telegram
open MyMarketBot.Moex

let INDEXES = [| "IMOEX"; "RTSI"; "RGBITR" |]

let prepareMessage (indexes: (string * IndexDayData)[]) =
    let indexes = indexes |> dict
    let toString = function
        | NoDataForADay -> "N/D"
        | Today today -> today.ToString()
        | TwoDays (today, yesterday) ->
            sprintf "%s (%+.2f%%)" (today.ToString()) (float((today - yesterday) / yesterday * 100M))
    let message index = sprintf "%s: %s" index (toString <| indexes.[index])
    
    StringBuilder()
        .Append(DateTime.Today.ToShortDateString()).AppendLine(" итоги дня:\n")
        .AppendJoin("\n", Array.map message INDEXES)
        .ToString()
        
let asyncWait async = (Async.StartAsTask async).Wait()

[<EntryPoint>]
let main _ =
    let token = Environment.GetEnvironmentVariable "TELEGRAM_BOT_TOKEN"
    let chatId = Environment.GetEnvironmentVariable "TELEGRAM_SUBSCRIBER_ID" |> Int64.Parse
   
    use bot = run token
    
    async {
        let! indexes =
                INDEXES
                |> Array.map (fun index -> (loadData index (DateTime.Now)))
                |> Async.Parallel
        let message = prepareMessage indexes
        do! send chatId message bot |> Async.Ignore
    } |> asyncWait
    0
