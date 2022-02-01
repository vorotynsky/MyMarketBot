module MyMarketBot.SpGlobal.SPX

open System
open MyMarketBot
open FSharp.Interop.Excel

type SpxProvider = ExcelFile<"""spx.xls""", Range="A7:B5000">

let readSpx (now : DateTime) =
    let provider = new SpxProvider()
    
    let row =
        provider.Data
        |> Seq.filter (fun x -> x.``S&P 500`` > 0.0)
        |> Seq.filter (fun x -> (now.Subtract x.``Effective date ``).Days < 5)
        |> List.ofSeq
        |> List.sortByDescending (fun x -> x.``Effective date ``)
        |> Seq.take 2
        |> Array.ofSeq
        
    row |> Market.makeData (fun x -> x.``Effective date ``) (fun x -> decimal x.``S&P 500``)
