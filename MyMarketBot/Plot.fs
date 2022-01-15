module MyMarketBot.Plot

open System.Diagnostics
open System.IO

let pyList values =
    let str = values |> Array.map (sprintf "%A") |> String.concat ", " |> sprintf "[%s]"
    str.Replace(";", ",")
       .Replace("M", "")
       .Replace("\n", "")
       .Replace("Some", "") // for optionals
         
let makeZcycScript (n, w, m) script =
    let substitute c d (s: string) =
        let name = "data_" + c + " = "
        s.Replace(name + "[]", name + pyList d)
    
    script
    |> substitute "m" m
    |> substitute "w" w
    |> substitute "0" n

let makeMosPrimeScript rows (script : string) =
    script.Replace("data = []", $"data = %s{pyList rows}")

let generateScript input output putData =
    File.ReadAllText(input)
    |> putData
    |> fun x -> File.WriteAllText(output, x)
    
let execute py scriptPath output =
    let info = ProcessStartInfo()
    info.FileName       <- py
    info.Arguments      <- $"%s{scriptPath} %s{output}"
    info.CreateNoWindow <- false
    
    Process.Start(info).WaitForExitAsync() |> Async.AwaitTask
