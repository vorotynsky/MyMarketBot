# My market bot

A telegram bot to monitor the state of the market.

Bot's message example:

```
02/05/2021 итоги дня:

🇷🇺 IMOEX: 3392.73 (📈 +0.61%)
🇷🇺 RTSI: 1431.76 (📈 +2.07%)
🇷🇺 RGBI: 151.16 (📈 +0.19%)
🇷🇺 RGBITR: 616.48 (📈 +0.20%)

📊 Spread index: -0.05% (📉 -0.41%)

💵 USD/RUB: 74.70 (📉 -1.28%) 
💶 EUR/RUB: 89.78 (📉 -0.86%)
```

This bot is a console application that sends a message to the specified user and shuts down. 

> The Moscow Exchange provides close values after 19:00 MSK.
> If you run this bot before 19:00 MSK or on weekends you will get yesterday values or N/D.

## Set up

Steps to run this bot:

1. Build the solution using `dotnet build`.
2. Set env-variables `TELEGRAM_BOT_TOKEN` and `TELEGRAM_SUBSCRIBER_ID`
3. Run F# project `dotnet run --project MyMarketBot`

Check [a GitHub action](https://github.com/vorotynsky/MyMarketBot/blob/master/.github/workflows/evening.yml) to see these steps.

### Arguments

Environment variables:

 - `TELEGRAM_BOT_TOKEN` - your bot's token. You can get it from Bot Father.
 - `TELEGRAM_SUBSCRIBER_ID` - your chat it. To get it run your bot in a debugger mode and send a message. Bot log your chat id to a console. 
