module MyMarketBot.Program

open System
open System.IO
open MyMarketBot.Telegram
open MyMarketBot.Moex
open MyMarketBot.Moex.Index
open MyMarketBot.Cbr

let asyncWait async = (Async.StartAsTask async).Wait()

let load f = Array.map (fun ticker -> f ticker DateTime.Now) >> Async.Parallel

let daily bot chatId = async {
    let! indexes = moexIndexes |> load Index.loadData
    let! currencies = [| "USD"; "EUR" |] |> load Currency.loadData

    let message = Message.prepareMessage indexes currencies
    do! send chatId message bot |> Async.Ignore
}

let zcyc bot chatId = async {
    let python = Environment.GetEnvironmentVariable "PYTHON_INTERPRETER"
    
    let! _, m = YieldCurve.loadZcycAround (DateTime.Now.AddMonths(-1))
    let! _, w = YieldCurve.loadZcycAround (DateTime.Now.AddDays(-7.0))
    let! _, n = YieldCurve.loadZcycAround (DateTime.Now)
    
    let! mos = MosPrime.readTable DateTime.Now

    let (~~) (str: string): string = Path.Join(AppDomain.CurrentDomain.BaseDirectory, str)
    do Plot.generateScript ~~"zcyc.py" ~~"zcyc_data.py" (Plot.makeZcycScript (n, w, m))
    do! Plot.execute python (Path.GetFileName ~~"./zcyc_data.py")
    
    do Plot.generateScript ~~"mosPrime.py" ~~"mosPrime_data.py" (Plot.makeMosPrimeScript mos)
    do! Plot.execute python (Path.GetFileName ~~"mosPrime_data.py")

    do! sendPicture chatId Message.zcyc ~~"zcyc.png" bot |> Async.Ignore
    do! sendPicture chatId Message.mosPrime ~~"mosprime.png" bot |> Async.Ignore
}

[<EntryPoint>]
let main args =
    
    let token = Environment.GetEnvironmentVariable "TELEGRAM_BOT_TOKEN"
    let chatId = Environment.GetEnvironmentVariable "TELEGRAM_SUBSCRIBER_ID" |> Int64.Parse
    
    let _daily = Array.contains "-daily" args
    let _zcyc  = Array.contains "-zcyc"  args
   
    use bot = run token
    
    async {
        do! if _daily then daily bot chatId else Common.fromResult ()
        do! if _zcyc  then zcyc  bot chatId else Common.fromResult ()
    } |> asyncWait
    
    #if DEBUG
    Console.WriteLine("Press any key to continue...")
    Console.ReadKey() |> ignore
    #endif
    
    0
