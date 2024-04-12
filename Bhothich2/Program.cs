using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Bhothich2;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("");
            client.StartReceiving(HandleUpdateAsync, Error);
            Console.ReadLine();
        }

        async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.Text != null)
            {
                await HandleMessage(botClient, update.Message);
                return;
            }

            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                await HandleCallbackQuerry(botClient, update.CallbackQuery);
                return;
            }
        }

        async static Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"{message.Chat.Username ?? "anonimus"}   |   {message.Text}");
            using (TestContext db = new TestContext())
            {
                if (db.Users.Any(u => u.telegramID.Contains(message.Chat.Username)))
                {
                    ReplyKeyboardMarkup keyboard = new(new[]
                            {
                        new KeyboardButton[] { "Правила", "Анкета цели" },
                        new KeyboardButton[] { "Сделать выстрел" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                    switch (message.Text)
                    {
                        case "/start":
                            
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Привет", replyMarkup: keyboard);
                            return;
                        case "Правила":
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Убиваешь, баллы получаешь. Умираешь, баллы теряешь.");
                            return;
                        case "bd":
                            //using (TestContext db = new TestContext())
                            //{
                            var users = db.Users.ToList();
                            foreach (var u in users)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"{u.Id}.{u.Name},{u.telegramID} - {u.Answers}, {u.Alive}, {u.Bullet}, {u.Points}");
                            }
                            //}
                            return;
                        case "Анкета цели":
                            //using (TestContext db = new TestContext())
                            //{
                            var target = db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).TargetId;
                            var answer = db.Users.FirstOrDefault(u => u.telegramID == target).Answers;
                            await botClient.SendTextMessageAsync(message.Chat.Id, answer);
                            //}
                            return;
                        case "Сделать выстрел":
                    //            ReplyKeyboardMarkup killboard = new(new[]
                    //                {
                    //    new KeyboardButton[] { "ДА", "НЕТ" }
                    //})
                    //            {
                    //                ResizeKeyboard = true
                    //            };
                            if (db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).Bullet == true)
                            { 
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Вы уверенны, что хотите зарядить пистолет?");
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Ответьте Да или Нет", replyMarkup: new ForceReplyMarkup { Selective = true });
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Вы уже стреляли в этом раунде");
                            }
                            //botClient.StartReceiving(HandleKillUpdateAsync, Error);
                            break;
                        
                    }
                    if (message.ReplyToMessage != null && message.ReplyToMessage.Text == "Ответьте Да или Нет")
                    {
                        switch (message.Text.ToUpper())
                        {
                            case "ДА":
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ФИО предпологаемой цели", replyMarkup: new ForceReplyMarkup { Selective = true });
                                break;
                            case "НЕТ":
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Вы отложили пистолет", replyMarkup: keyboard);
                                break;
                        }
                    }

                    if (message.ReplyToMessage != null && message.ReplyToMessage.Text == "Введите ФИО предпологаемой цели")
                    {

                        db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).Bullet = false;
                        db.SaveChanges();
                        if (db.Users.FirstOrDefault(u => u.telegramID == db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).TargetId).Name == message.Text)
                        {
                            if (db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).Alive == true)
                            {
                                DateTime timeofdeath = DateTime.Now;
                                db.Users.FirstOrDefault(u => u.telegramID == db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).TargetId).TimeOfDeath = timeofdeath;
                                db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).TimeOfShot = timeofdeath;
                                db.Users.FirstOrDefault(u => u.telegramID == db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).TargetId).Alive = false;
                                db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).CorrectOfShot = true;
                                db.SaveChanges();
                            }
                            if (db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).Alive == false)
                            {
                                DateTime timeofdeath = Convert.ToDateTime(DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss"));
                                db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).TimeOfShot = timeofdeath;
                                db.Users.FirstOrDefault(u => u.telegramID == message.Chat.Username).CorrectOfShot = true;
                            }
                        }
                        else
                        {
                            //придумать чё изменять.
                        }
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы стрельнули в {message.Text}, на этом пока всё...", replyMarkup: keyboard);
                    }
                }



                        if (message.Chat.Username == "xmasstree")
                {
                    switch (message.Text)
                    {
                        case "/start":
                            ReplyKeyboardMarkup keyboard = new(new[]
                                {
                        new KeyboardButton[] { "Статистика", "Выгрузить Excel" },
                        new KeyboardButton[] { "Поставить таймер раунда" }
                    })
                            {
                                ResizeKeyboard = true
                            };
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Здарова, Бандиты!", replyMarkup: keyboard);
                            break;
                        case "Статистика":
                            
                                var countA = db.Users.Count(u => u.Alive == true);
                                var countD = db.Users.Count(u => u.Alive == false);
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"Живых:{countA}  |  Не живых:{countD}");
                                break;
                            
                    }
                }
            }


        }
        
        async static Task HandleCallbackQuerry(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {

        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
















//var message = update.Message;
//if (message.Text != null)
//{
//    Console.WriteLine($"{message.Chat.Username ?? "anonimus"}   |   {message.Text}");
//    if (message.Text.ToLower().Contains("приколист"))
//    {
//        await botClient.SendTextMessageAsync(message.Chat.Id, "Да");
//        return;
//    }
//    if (message.Text.ToLower().Contains("абоба"))
//    {
//        await botClient.SendTextMessageAsync(message.Chat.Id, "Сам абоба!");
//        return;
//    }
//    if (message.Text.ToLower().Contains("bd"))
//    {
//        using (TestContext db = new TestContext())
//        {
//            var users = db.Users.ToList();
//            foreach (var u in users) 
//            {
//                await botClient.SendTextMessageAsync(message.Chat.Id, $"{u.Id}.{u.Name} - {u.Age}");
//            }
//        }
//        return;

//    }
//}
//if (message.Photo != null)
//{
//    await botClient.SendTextMessageAsync(message.Chat.Id, "Красивое! А можно документом скинуть?");
//    return;
//}

////if (message.Document != null)
////{
////    await botClient.SendTextMessageAsync(message.Chat.Id, "Чё-то фотошопит...");

////    var fileId = update.Message.Document.FileId;
////    var fileInfo = await botClient.GetFileAsync(fileId);
////    var filePath = fileInfo.FilePath;

////    string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{message.Document.FileName}";
////    await using Stream fileStream = System.IO.File.OpenWrite(destinationFilePath);
////    await botClient.DownloadFileAsync(
////        filePath,
////        fileStream);
////    fileStream.Close();

////    Process.Start(@"C:\Users\Илья\Desktop\Жмых.exe", $@"""{destinationFilePath}""");
////    await Task.Delay(10000);

////    await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
////    await botClient.SendDocumentAsync(
////        message.Chat.Id,
////        new InputOnlineFile(stream, message.Document.FileName.Replace(".jpg", "(edited).jpg")));
////    return;
////}
