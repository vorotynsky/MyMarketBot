module MyMarketBot.Moex

open System
open FSharp.Data

[<Literal>]
let private indexUrl =
    """https://iss.moex.com/iss/history/engines/stock/markets/index/sessions/total/securities/RTSI.xml?from=2001-10-15&till=2001-10-16&iss.meta=off&iss.cursor=off&history.columns=%20SECID,TRADEDATE,CLOSE&iss.only=history"""

type MoexIndexProvider = XmlProvider<indexUrl>

type IndexDayData =
    | NoDataForADay
    | Today of decimal
    | TodayWithDailyChange of today: decimal * change: decimal

let makeUrl index (startDate: DateTime) (endDate: DateTime) =
    let dateFormat = "yy-MM-dd"

    indexUrl
        .Replace("RTSI", index)
        .Replace("2001-10-15", startDate.ToString(dateFormat))
        .Replace("2001-10-16", endDate.ToString(dateFormat))

let loadData index (now: DateTime) =
    async {
        let yesterday =
            if DateTime.Today.DayOfWeek = DayOfWeek.Monday
            then 3.0
            else 1.0
            |> TimeSpan.FromDays
            |> now.Subtract

        let! rowData =
            makeUrl index yesterday now
            |> Http.AsyncRequestString
            
        let data = MoexIndexProvider.Parse rowData

        return match data.Data.Rows |> Array.map (fun x -> x.Close) with
               | [| today |] -> Today today
               | [| yesterday; today |] -> TodayWithDailyChange (today, (today - yesterday) / yesterday)
               | _ -> NoDataForADay
    }
