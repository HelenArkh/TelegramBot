using System.Data;
using System.Data.SqlClient;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class BotWorker
    {
        private ITelegramBotClient botClient;
        DatabaseQueries databaseQueries = new DatabaseQueries();
        

        public void Inizalize()
        {
            botClient = new TelegramBotClient(BotCredentials.BotToken);
        }

        public void Start()
        {
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
        }

        public void Stop()
        {
            botClient.StopReceiving();
        }

        /* В проекте использовала статусную модель и таблицу Status 
         CREATE TABLE [dbo].[Status] (
    [ChatId] INT NOT NULL,
    [Status] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ChatId] ASC)
);
        Статусы:
        0 - отсутствие записи в таблице
        1 - ожидание русского ввода слова
        2 - ожидание ввода английского слова
        3 - ожидание ввода тематики
        4 - успешное завершение операции и ожидание любой дальнейшей команды
        5 - ожидание ввода слова на удаление
        6 - ожидание ввода тематики тренировки
        7 - ожидание ввода языка, с какого делать перевод во время тренировки
        8 - режим тренировки
         
         */

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message != null)
            {
                Message msg = e.Message; //создаём переменную, которая содержит все свойства сообщения
                int status = databaseQueries.SelectStatusByChatId(msg.Chat.Id);

                
                    if (msg.Text == "/addword" && status == 0)
                    {
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Введите русское значение слова");

                    databaseQueries.InsertNewStatus(msg.Chat.Id);
                    }
                    
                    if (msg.Text != "/addword" && msg.Text != "/deleteword" && status == 1)
                    {
                    databaseQueries.InsertNewWord(msg.Text, msg.Chat.Id);

                        await botClient.SendTextMessageAsync(
           chatId: e.Message.Chat, text: "Введите английское значение слова");

                    databaseQueries.UpdateStatus(2, msg.Chat.Id);
                    }

                    if (msg.Text != "/addword" && msg.Text != "/deleteword" && status == 2)
                    {
                    databaseQueries.AddEngWordToWordTable(msg.Text, msg.Chat.Id);

                        await botClient.SendTextMessageAsync(
           chatId: e.Message.Chat, text: "Введите тематику");

                    databaseQueries.UpdateStatus(3, msg.Chat.Id);                    
                    }

                    if (msg.Text != "/addword" && msg.Text != "/deleteword" && status == 3)
                    {

                    databaseQueries.AddCategoryToWordTable(msg.Text, msg.Chat.Id);

                        await botClient.SendTextMessageAsync(
           chatId: e.Message.Chat, text: $"Успешно! Слово добавлено в словарь.");

                    databaseQueries.UpdateStatus(4, msg.Chat.Id);                   
                    }

                    if (msg.Text == "/addword" && status == 4)
                    {
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Введите русское значение слова");

                    databaseQueries.UpdateStatus(1, msg.Chat.Id);                    
                    }

                    if (msg.Text == "/deleteword" && status == 4)
                    {
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Введите английское слово, которое следует удалить");

                    databaseQueries.UpdateStatus(5, msg.Chat.Id);                    
                    }

                    if (msg.Text == "/deleteword" && status == 0)
                    {
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: @"Вам еще пока нечего удалять, т.к. Вы ничего не сохранили в словарь. Для сохранения воспользуйтесь командой /addword");

                    }

                    if (msg.Text != "/addword" && msg.Text != "/deleteword" && status == 5)
                    {
                    databaseQueries.DeleteWord(msg.Text, msg.Chat.Id);                        

                        await botClient.SendTextMessageAsync(
           chatId: e.Message.Chat, text: $"Успешно! Слово {msg.Text} удалено из словаря.");

                    databaseQueries.UpdateStatus(4, msg.Chat.Id);                    
                    }

                    if (msg.Text == "/startnow" && status == 4)
                    {
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Тренировку по какой тематике Вы хотите начать? Напишите тематику слов или все (тогда запустится тренировка по всем тематикам)");

                    databaseQueries.UpdateStatus(6, msg.Chat.Id);                    
                    }

                    if (msg.Text == "/startnow" && status == 0)
                    {
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: @"Вам еще пока нечего тренировать, т.к. Вы ничего не сохранили в словарь. Для сохранения воспользуйтесь командой /addword");

                    }

                    if (status == 6)
                    {
                    string category = databaseQueries.SelectCategoryFromWord(msg.Chat.Id, msg.Text);                        

                        if (category == "NULL")
                        {
                            await botClient.SendTextMessageAsync(
           chatId: e.Message.Chat, text: $"Указанной тематики {msg.Text} не существует. Попробуйте написать еще");
                        }
                        else
                        {
                        databaseQueries.InsertNewWorkout(msg.Text, msg.Chat.Id);
                                
                            
                            
                            await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Перевод с какого языка будем делать? Русский или английский?");

                        databaseQueries.UpdateStatus(7, msg.Chat.Id);
                    }
                    }

                    if (status == 7)
                    {
                        string language = msg.Text;

                        if (language != "русский" && language != "английский")
                        {
                            await botClient.SendTextMessageAsync(
           chatId: e.Message.Chat, text: $"Тренировки на языке {msg.Text} не существует. Только английский или русский");
                        }
                        else
                        {
                        databaseQueries.AddTranslateToWorkoutTable(msg.Text, msg.Chat.Id);

                        

                            await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Тренировка начата");

                        string category = databaseQueries.SelectCategoryFromWorkout(msg.Chat.Id);
                        string translate = databaseQueries.SelectTranslateFromWorkout(msg.Chat.Id);
                        string word = "NULL";                            

                            if (translate == "английский")
                            {
                            word = databaseQueries.SelectRandomEngWordFromWord(msg.Chat.Id, category);

                            databaseQueries.AddWordToWorkoutTable(word, msg.Chat.Id);    
                            }

                            if (translate == "русский")
                            {
                            word = databaseQueries.SelectRandomRusWordFromWord(msg.Chat.Id, category);

                            databaseQueries.AddWordToWorkoutTable(word, msg.Chat.Id);
                            }

                            await botClient.SendTextMessageAsync(
       chatId: e.Message.Chat, text: $"{word}");

                        databaseQueries.UpdateStatus(8, msg.Chat.Id);
                    }
                    }

                    if (msg.Text != "/stop" && status == 8)
                    {
                    string category = databaseQueries.SelectCategoryFromWorkout(msg.Chat.Id);
                    string translate = databaseQueries.SelectTranslateFromWorkout(msg.Chat.Id);
                    string word = databaseQueries.SelectWordFromWorkout(msg.Chat.Id);

                    string firstWord = "NULL";
                        string secondWord = "NULL";
                        
                        if (translate == "английский")
                        {
                        firstWord = databaseQueries.SelectRusWordByEngWord(msg.Chat.Id, word);

                        secondWord = databaseQueries.SelectRandomEngWordFromWord(msg.Chat.Id, category);

                        databaseQueries.AddWordToWorkoutTable(secondWord, msg.Chat.Id);                       
                        }

                        if (translate == "русский")
                        {
                        firstWord = databaseQueries.SelectEngWordByRusWord(msg.Chat.Id, word);

                        secondWord = databaseQueries.SelectRandomRusWordFromWord(msg.Chat.Id, category);

                        databaseQueries.AddWordToWorkoutTable(secondWord, msg.Chat.Id);
                        }
                        
                        if(msg.Text == firstWord)
                        {
                            await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Правильно!");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: $"Неправильный перевод. Правильно так: {firstWord}");
                        }
                        
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: $"{secondWord}");
                    }


                    if (msg.Text == "/stop" && status == 8)
                    {
                        await botClient.SendTextMessageAsync(
            chatId: e.Message.Chat, text: "Тренировка завершена");

                    databaseQueries.DeleteWorkout(msg.Chat.Id);

                    databaseQueries.UpdateStatus(4, msg.Chat.Id);                    
                    }               
            }
        }
    }
}