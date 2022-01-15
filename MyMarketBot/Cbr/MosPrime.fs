module MyMarketBot.Cbr.MosPrime

open System
open FSharp.Data

let formatDate (date: DateTime) = date.ToString "dd.MM.yyyy"

[<Literal>]
let url = """https://www.cbr.ru/eng/hd_base/mosprime//?UniDbQuery.Posted=True&UniDbQuery.From=17.04.2020&UniDbQuery.To=17.05.2020"""

let replaceUrl f s =
    url.Replace("17.04.2020", formatDate f)
       .Replace("17.05.2020", formatDate s)

type SiteMosPrimeProvider = HtmlProvider<Sample=url, PreferOptionals=true, MissingValues="—">

let readTable (now : DateTime) = async {
    
    let! page =
        replaceUrl (now.AddMonths -1) now
        |> SiteMosPrimeProvider.AsyncLoad
        
    let data = page.Tables.Table1.Rows
    do data |> Array.sortInPlaceBy (fun x -> DateTime.ParseExact(x.Date |> Option.get, "dd.MM.yyyy", null))
  
    return data
}
   