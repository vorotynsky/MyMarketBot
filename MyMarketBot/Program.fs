module MyMarketBot.Program

open System
open System.Text
open MyMarketBot.Telegram
open MyMarketBot.Moex

let INDEXES = [| "IMOEX"; "RTSI"; "RGBITR" |]

let prepareMessage (indexes: (string * IndexDayData)[]) =
    let indexes = indexes |> dict
    let toString formatter = function
        | NoDataForADay -> "N/D"
        | Today today -> sprintf "%s" (formatter today)
        | TwoDays (today, yesterday) ->
            sprintf "%s (%+.2f%%)" (formatter today) (float((today - yesterday) / yesterday * 100M))
    let message index = sprintf "%s: %s" index (toString (sprintf "%.2f") indexes.[index])
    
    let safeIndex =
        let calculate imoex rgbitr = rgbitr / imoex
        match (indexes.["IMOEX"], indexes.["RGBITR"]) with
        | (TwoDays (it, iy), TwoDays (bt, by)) -> TwoDays ((calculate it bt), (calculate iy by))
        | (HasToday i, HasToday b) -> Today (calculate i b)
        | _ -> NoDataForADay
    
    StringBuilder()
        .Append(DateTime.Today.ToShortDateString()).AppendLine(" итоги дня:\n")
        .AppendJoin("\n", Array.map message INDEXES).Append("\n\n")
        .Append("Spread index: ").AppendLine(toString (fun x -> sprintf "%.2f%%" << float <| (x - 0.184M)/(0.233M - 0.184M)) safeIndex)
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
    
    #if DEBUG
    Console.WriteLine("Press any key to continue...")
    Console.ReadKey() |> ignore
    #endif
    
    0
