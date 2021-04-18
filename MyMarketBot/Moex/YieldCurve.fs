module MyMarketBot.Moex.YieldCurve

open FSharp.Data

[<Literal>]
let url = """https://iss.moex.com/iss/engines/stock/zcyc.xml?iss.only=yearyields&iss.meta=off&yearyields.columns=period,value&date=2021-04-14"""

type ZcycProvider = XmlProvider<url>

let makeUrl now = url.Replace ("2021-04-14", Common.formatDate now)

let loadZcyc now = async {
    let! data = ZcycProvider.AsyncLoad (makeUrl now)
    return data.Data.Rows |> Array.map (fun x -> (x.Period, x.Value))
}

let fromResult x = async { return x }

let rec loadZcycAround now = async {
    let! data = loadZcyc now
    
    return!
        if data.Length = 0
        then loadZcycAround (Common.yesterday now)
        else fromResult (now, data)
}
