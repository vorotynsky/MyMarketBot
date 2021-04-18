module MyMarketBot.Program

open System
open System.IO
open MyMarketBot.Telegram
open MyMarketBot.Moex
open MyMarketBot.Moex.Index

let asyncWait async = (Async.StartAsTask async).Wait()

let load f = Array.map (fun ticker -> f ticker DateTime.Now) >> Async.Parallel
    

[<EntryPoint>]
let main _ =
    let token = Environment.GetEnvironmentVariable "TELEGRAM_BOT_TOKEN"
    let chatId = Environment.GetEnvironmentVariable "TELEGRAM_SUBSCRIBER_ID" |> Int64.Parse
    let python = Environment.GetEnvironmentVariable "PYTHON_INTERPRETER"
   
    use bot = run token
    
    async {
        let! indexes = moexIndexes |> load Index.loadData
        let! currencies = [| "USD"; "EUR" |] |> load Currency.loadData
        
        let message = Message.prepareMessage indexes currencies
        do! send chatId message bot |> Async.Ignore
        
        let! _, m = YieldCurve.loadZcycAround (DateTime.Now.AddMonths(-1))
        let! _, w = YieldCurve.loadZcycAround (DateTime.Now.AddDays(-7.0))
        let! _, n = YieldCurve.loadZcycAround (DateTime.Now)
        
        do Plot.generateScript "./plot.py" "./plot_data.py" (n, w, m)
        do! Plot.execute python (Path.GetFileName "./plot_data.py")
        
        do! sendPicture chatId Message.zcyc "./zcyc.png" bot |> Async.Ignore
    } |> asyncWait
    
    #if DEBUG
    Console.WriteLine("Press any key to continue...")
    Console.ReadKey() |> ignore
    #endif
    
    0
