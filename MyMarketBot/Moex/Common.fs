module MyMarketBot.Moex.Common

open System

let formatDate (date: DateTime) = date.ToString "yy-MM-dd"

let makeUrlBuilder (url: string) (_index: string) _start _end index startDate endDate =
    url.Replace(_index, index)
       .Replace(_start, formatDate startDate)
       .Replace(_end, formatDate endDate)
       
let fromResult x = async { return x }
