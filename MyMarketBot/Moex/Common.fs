module MyMarketBot.Moex.Common

open System

type MoexDayData =
    | NoDataForADay
    | Today of decimal
    | TwoDays of today: decimal * yesterday: decimal

let (|HasToday|_|) = function
    | NoDataForADay -> None
    | Today today -> Some today
    | TwoDays (today, _) -> Some today

let makeUrlBuilder (url: string) (_index: string) _start _end index startDate endDate =
    let formatDate (date: DateTime) = date.ToString "yy-MM-dd"
    url.Replace(_index, index)
       .Replace(_start, formatDate startDate)
       .Replace(_end, formatDate endDate)
       
let yesterday (now: DateTime) =
    if now.DayOfWeek = DayOfWeek.Monday then 3.0 else 1.0
    |> TimeSpan.FromDays
    |> now.Subtract
    
let makeData = function
    | [| today |] -> Today today
    | [| today; yesterday |] -> TwoDays (today, yesterday)
    | _ -> NoDataForADay
