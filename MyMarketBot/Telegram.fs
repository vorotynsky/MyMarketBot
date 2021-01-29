namespace MyMarketBot

open System
open Telegram.Bot
open Telegram.Bot.Types

type Bot(bot: ITelegramBotClient) =
    let helloMessage firstName lastName chatId =
        let name = (firstName + (if (lastName = null && firstName <> null) then "" else " ") + lastName)
    #if DEBUG
        printfn "User %s with ChatId %d" name chatId
        sprintf "Hello %s, i logged your chat id to my console." name 
    #else
        sprintf "Hello %s, it is my private bot. You can check my github repo with it (https://github.com/vorotynsky/MyMarketBot)" name
    #endif
    
    
    do bot.StartReceiving()
    
    do bot.OnMessage.Add(fun evArgs ->
            let chat = evArgs.Message.Chat
            bot.SendTextMessageAsync(ChatId(chat.Id), helloMessage chat.FirstName chat.LastName chat.Id)
            |> Async.AwaitTask
            |> Async.Ignore
            |> Async.Start)
    
    member _.Bot = bot
    interface IDisposable with
        member this.Dispose() = bot.StopReceiving()
        
module Telegram = 

    let run token = new Bot(TelegramBotClient token)

    let send (chatId: int64) message  (bot: Bot) =
        bot.Bot.SendTextMessageAsync(ChatId chatId, message) |> Async.AwaitTask
