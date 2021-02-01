module MyMarketBot.Message

open System
open System.Text
open MyMarketBot.Moex.Common
open MyMarketBot.Moex.Index

let trendIcon change = if (change >= 0.0) then "📈" else "📉"

let toString formatter = function
        | NoDataForADay -> "N/D"
        | Today today -> sprintf "%s" (formatter today)
        | TwoDays (today, yesterday) ->
            let change = float((today - yesterday) / yesterday * 100M)
            sprintf "%s (%s %+.2f%%)" (formatter today) (trendIcon change) change
          

let prepareMessage (indexes: (string * MoexDayData)[]) (currencies: (string * MoexDayData)[]) =
    let indexes = dict indexes 
    let currencies = dict currencies
    let message icon index = sprintf "%s %s: %s" icon index (toString (sprintf "%.2f") indexes.[index])
    
    let builder = StringBuilder()
    let sbprintf format = Printf.bprintf builder format
    
    
    sbprintf "%s итоги дня:\n\n" (DateTime.Today.ToShortDateString())
    
    builder.AppendJoin("\n", Array.map (message "🇷🇺") moexIndexes) |> ignore
    
    sbprintf "\n\n📊 Spread index: %s"
        (toString
             (sprintf "%.2f%%" << rangeSpreadIndex)
             (spreadIndex indexes.["IMOEX"] indexes.["RGBITR"]))
        
    sbprintf "\n\n💵 USD/RUB: %s \n💶 EUR/RUB: %s"
        (toString (sprintf "%.2f") currencies.["USD"])
        (toString (sprintf "%.2f") currencies.["EUR"])

    builder.ToString()
