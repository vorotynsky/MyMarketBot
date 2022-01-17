module MyMarketBot.Market

open System

type MarketDayData =
    | NoDataForADay
    | Today of decimal
    | TwoDays of today: decimal * yesterday: decimal

let (|HasToday|_|) = function
    | NoDataForADay -> None
    | Today today -> Some today
    | TwoDays (today, _) -> Some today
    
let yesterday (now: DateTime) =
    if now.DayOfWeek = DayOfWeek.Monday then 3.0 else 1.0
    |> TimeSpan.FromDays
    |> now.Subtract
    
let makeData s m = Array.sortBy s >> Array.map m >> function
    | [| today |] -> Today today
    | [| yesterday; today |] -> TwoDays (today, yesterday)
    | _ -> NoDataForADay

let zipWith f first second =
    match (first, second) with
    | (TwoDays (ta, ya), TwoDays (tb, yb)) -> TwoDays (f ta tb, f ya yb)
    | (HasToday a, HasToday b) -> Today (f a b)
    | _ -> NoDataForADay
