module MyMarketBot.Message

open System
open System.Text
open Microsoft.FSharp.Core
open MyMarketBot.Market
open MyMarketBot.Moex.Index

let trendIcon change = if (change >= 0.0) then "📈" else "📉"

let toString formatter = function
        | NoDataForADay -> "N/D"
        | Today today -> sprintf "%s" (formatter today)
        | TwoDays (today, yesterday) ->
            let change = float((today - yesterday) / yesterday * 100M)
            sprintf "%s (%s %+.2f%%)" (formatter today) (trendIcon change) change
            
type MessageInfo =
    | IndexMessage of string * string * MarketDayData
    | TextMessage  of string
    | JoinedMessage of MessageInfo[]

let rec singleMessage (sb: StringBuilder) = function
    | TextMessage text -> Printf.bprintf sb $"%s{text}\n"
    | IndexMessage(icon, name, info) -> Printf.bprintf sb "%s %s: %s\n" icon name (toString (sprintf "%.2f") info)
    | JoinedMessage infos -> Array.iter (singleMessage sb) infos

let prepareMessages (messages: MessageInfo[]) =
    let sb = StringBuilder()
    Array.iter (singleMessage sb) messages
    sb.ToString()
    

let moexMessage (indexes: (string * MarketDayData)[]) (currencies: (string * MarketDayData)[]) =
    let indexes = dict indexes 
    let currencies = dict currencies
    
    prepareMessages [|
        TextMessage $"%s{DateTime.Today.ToShortDateString()} итоги дня:\n"
        
        JoinedMessage (Array.map (fun x -> IndexMessage ("🇷🇺", x, indexes.[x])) moexIndexes)
        
        TextMessage "";
        IndexMessage ("📊", "Spread index", spreadIndex indexes.["IMOEX"] indexes.["RGBITR"])
        
        TextMessage "";
        IndexMessage ("💵", "USD/RUB", currencies.["USD"])
        IndexMessage ("💶", "EUR/RUB", currencies.["EUR"])
    |]

let spxMessage spx =
    prepareMessages [|
        TextMessage $"%s{DateTime.Today.Subtract(TimeSpan.FromDays 1).ToShortDateString()} итоги дня:\n"
        
        IndexMessage ("🇺🇸", "S&P 500", spx)
    |]

let zcyc = "🇷🇺 Кривая доходности ОФЗ, " + DateTime.Today.ToShortDateString()
let mosPrime = "🇷🇺 MosPrime Rate, " + DateTime.Today.ToShortDateString()
