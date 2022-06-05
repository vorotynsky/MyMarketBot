module MyMarketBot.Moex.Currency

open FSharp.Data
open MyMarketBot.Market
open MyMarketBot.Moex.Common

[<Literal>]
let currencyUrl =
    """https://iss.moex.com/iss/statistics/engines/futures/markets/indicativerates/securities/USD/RUB.xml?from=2020-01-08&till=2020-01-09&iss.meta=off&iss.cursor=off&securities.columns=secid,tradedate,rate&iss.cursor=off&iss.only=securities"""
    
type MoexCurrencyProvider = XmlProvider<"Data/USDRUB.xml">

let makeUrl = makeUrlBuilder currencyUrl "USD" "2020-01-08" "2020-01-09"

let loadData currency now =
    async {
        let! data =
            makeUrl currency (yesterday now) now
            |> MoexCurrencyProvider.AsyncLoad
        
        return (currency, data.Data.Rows |> makeData (fun x -> x.Tradedate) (fun x -> x.Rate))
    }
