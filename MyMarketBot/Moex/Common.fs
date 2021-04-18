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

let formatDate (date: DateTime) = date.ToString "yy-MM-dd"

let makeUrlBuilder (url: string) (_index: string) _start _end index startDate endDate =
    url.Replace(_index, index)
       .Replace(_start, formatDate startDate)
       .Replace(_end, formatDate endDate)
       
let yesterday (now: DateTime) =
    if now.DayOfWeek = DayOfWeek.Monday then 3.0 else 1.0
    |> TimeSpan.FromDays
    |> now.Subtract
    
let makeData s m = Array.sortBy s >> Array.map m >> function
    | [| today |] -> Today today
    | [| yesterday; today |] -> TwoDays (today, yesterday)
    | _ -> NoDataForADay
