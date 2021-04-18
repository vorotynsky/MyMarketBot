module MyMarketBot.Plot

open System.Diagnostics
open System.IO

let pyList values =
    let str = values |> List.ofArray |> sprintf "%A" 
    str.Replace(";", ",")
       .Replace("M", "")
       .Replace("\n", "")
         
let makePlotScript (n, w, m) script =
    let substitute c d (s: string) =
        let name = "data_" + c + " = "
        s.Replace(name + "[]", name + pyList d)
    
    script
    |> substitute "m" m
    |> substitute "w" w
    |> substitute "0" n


let generateScript input output data =
    File.ReadAllText(input)
    |> makePlotScript data
    |> fun x -> File.WriteAllText(output, x)
    
let execute py scriptPath =
    let info = ProcessStartInfo()
    info.FileName       <- py
    info.Arguments      <- scriptPath
    info.CreateNoWindow <- false
    
    Process.Start(info).WaitForExitAsync() |> Async.AwaitTask
