module MyMarketBot.Message

open System
open System.Text
open MyMarketBot.Moex.Common
open MyMarketBot.Moex.Index

let toString formatter = function
        | NoDataForADay -> "N/D"
        | Today today -> sprintf "%s" (formatter today)
        | TwoDays (today, yesterday) ->
            sprintf "%s (%+.2f%%)" (formatter today) (float((today - yesterday) / yesterday * 100M))
          

let prepareMessage (indexes: (string * MoexDayData)[]) =
    let indexes = indexes |> dict
    let message index = sprintf "%s: %s" index (toString (sprintf "%.2f") indexes.[index])
    
    let builder = StringBuilder()
    let sbprintf = Printf.bprintf builder
    
    
    sbprintf "%s итоги дня:\n\n" (DateTime.Today.ToShortDateString())
    
    builder.AppendJoin("\n", Array.map message moexIndexes) |> ignore
    
    sbprintf "\n\n Spread index: %s"
        (toString
             (sprintf "%.2f%%" << rangeSpreadIndex)
             (spreadIndex indexes.["IMOEX"] indexes.["RGBITR"]))

    builder.ToString()
