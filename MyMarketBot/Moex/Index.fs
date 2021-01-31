module MyMarketBot.Moex.Index

open System
open FSharp.Data
open MyMarketBot.Moex.Common

let moexIndexes = [| "IMOEX"; "RTSI"; "RGBITR" |]

[<Literal>]
let private indexUrl =
    """https://iss.moex.com/iss/history/engines/stock/markets/index/sessions/total/securities/RTSI.xml?from=2001-10-15&till=2001-10-16&iss.meta=off&iss.cursor=off&history.columns=%20SECID,TRADEDATE,CLOSE&iss.only=history"""

type MoexIndexProvider = XmlProvider<indexUrl>

let makeUrl = makeUrlBuilder indexUrl "RTSI" "2001-10-15" "2001-10-16"

let loadData index now =
    async {
        let! data =
            makeUrl index (yesterday now) now
            |> MoexIndexProvider.AsyncLoad

        return (index, data.Data.Rows |> Array.map (fun x -> x.Close) |> makeData)
    }
    
let spreadIndex imoex rgbitr =
    let calculate imoex rgbitr = rgbitr / imoex
    match (imoex, rgbitr) with
    | (TwoDays (it, iy), TwoDays (bt, by)) -> TwoDays ((calculate it bt), (calculate iy by))
    | (HasToday i, HasToday b)             -> Today    (calculate i  b)
    | _                                    -> NoDataForADay

let rangeSpreadIndex x = (x - 0.184M) / (0.233M - 0.184M)
