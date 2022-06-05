module MyMarketBot.Moex.Index

open FSharp.Data
open MyMarketBot.Market
open MyMarketBot.Moex.Common

let moexIndexes = [| "IMOEX"; "RTSI"; "RGBI"; "RGBITR" |]

[<Literal>]
let private indexUrl =
    """https://iss.moex.com/iss/history/engines/stock/markets/index/sessions/total/securities/RTSI.xml?from=2001-10-15&till=2001-10-16&iss.meta=off&iss.cursor=off&history.columns=%20SECID,TRADEDATE,CLOSE&iss.only=history"""

type MoexIndexProvider = XmlProvider<"Data/RTSI.xml">

let makeUrl = makeUrlBuilder indexUrl "RTSI" "2001-10-15" "2001-10-16"

let loadData index now =
    async {
        let! data =
            makeUrl index (yesterday now) now
            |> MoexIndexProvider.AsyncLoad

        return (index, data.Data.Rows |> makeData (fun x -> x.Tradedate) (fun x -> x.Close))
    }
    
let spreadIndex imoex rgbitr = zipWith (/) imoex rgbitr

let rangeSpreadIndex x = (x - 0.184M) / (0.233M - 0.184M)
