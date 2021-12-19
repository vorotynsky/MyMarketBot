module MyMarketBot.Cbr.MosPrime

open System
open FSharp.Data

let formatDate (date: DateTime) = date.ToString "dd.MM.yyyy"

[<Literal>]
let url = """https://www.cbr.ru/eng/hd_base/mosprime//?UniDbQuery.Posted=True&UniDbQuery.From=05.11.2021&UniDbQuery.To=17.12.2021"""

let replaceUrl f s =
    url.Replace("05.11.2021", formatDate f)
       .Replace("17.12.2021", formatDate s)

type SiteMosPrimeProvider = HtmlProvider<url>

let readTable (now : DateTime) = async {
    
    let! page =
        replaceUrl (now.AddMonths -1) now
        |> SiteMosPrimeProvider.AsyncLoad
        
    let data = page.Tables.Table1.Rows
    do data |> Array.sortInPlaceBy (fun x -> DateTime.ParseExact(x.Date, "dd.MM.yyyy", null))
  
    return data
}
   