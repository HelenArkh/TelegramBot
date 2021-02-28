using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TelegramBot
{
    public class DatabaseQueries
    {
        static SqlConnection sql = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=D:\VS_Projects\TelegramBot\TelegramBot\TelegramBot\Database1.mdf;Integrated Security = True");
        

        public int SelectStatusByChatId(long chatId)
        {
            int status = 0;
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                
                sql.Open(); //Открываем sql подключение
                SqlCommand commandStatus = new SqlCommand($"select Status from Status where ChatId = '{chatId}'", sql);
                SqlDataReader reader = commandStatus.ExecuteReader();

                while (reader.Read())
                {
                    status = (int)reader["Status"];
                }
                sql.Close();

            }

            return status;
        }

        public async void InsertNewStatus(long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand command = new SqlCommand($"insert into Status (ChatId, Status) values ({chatId}, 1)", sql);
                await command.ExecuteNonQueryAsync(); //Выполняем команду
                sql.Close();
            }
        }

        public async void InsertNewWord(string rusWord, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand command = new SqlCommand($"insert into Word (wordId, rusWord, chatId) values ((SELECT 1 + MAX(wordId) FROM Word), N'{rusWord}', {chatId})", sql);
                await command.ExecuteNonQueryAsync(); //Выполняем команду
                sql.Close();
            }
        }

        public async void UpdateStatus(int number, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand command = new SqlCommand($"update Status set Status = {number} where ChatId = {chatId}", sql);
                await command.ExecuteNonQueryAsync(); //Выполняем команду
                sql.Close();
            }
        }

        public async void AddEngWordToWordTable(string engWord, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand command = new SqlCommand($"update Word set engWord = '{engWord}' where ChatId = {chatId} and engWord is NULL", sql);

                await command.ExecuteNonQueryAsync(); //Выполняем команду
                sql.Close();
            }
        }

        public async void AddCategoryToWordTable(string category, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand command = new SqlCommand($"update Word set Category = N'{category}' where ChatId = {chatId} and engWord is not NULL and Category is NULL", sql);
                await command.ExecuteNonQueryAsync(); //Выполняем команду                                       
                sql.Close();
            }
        }

        public async void DeleteWord(string engWord, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand command = new SqlCommand($"delete from Word where engWord = '{engWord}' and ChatId = {chatId}", sql);
                await command.ExecuteNonQueryAsync(); //Выполняем команду                                       
                sql.Close();
            }
        }

        public string SelectCategoryFromWord(long chatId, string categoryFromText)
        {
            string category = "NULL";
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand searchCategory = new SqlCommand($"select top (1) Category from Word where ChatId = '{chatId}' and Category = N'{categoryFromText}'", sql);
                SqlDataReader search = searchCategory.ExecuteReader();

                while (search.Read())
                {
                    category = (string)search["Category"];
                }
                sql.Close();
            }

            return category;
        }

        public async void InsertNewWorkout(string category, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand addWorkout = new SqlCommand($"insert into Workout (Id, category, chatId) values ((SELECT 1 + MAX(wordId) FROM Word), N'{category}', {chatId})", sql);
                await addWorkout.ExecuteNonQueryAsync(); //Выполняем команду                
                sql.Close();
            }
        }

        public async void AddTranslateToWorkoutTable(string translate, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {               
                sql.Open(); //Открываем sql подключение
                SqlCommand addWorkout = new SqlCommand($"update Workout set translate = N'{translate}' where ChatId = {chatId} and translate is NULL", sql);
                await addWorkout.ExecuteNonQueryAsync(); //Выполняем команду                                      
                sql.Close();
            }
        }

        public string SelectCategoryFromWorkout(long chatId)
        {
            string category = "NULL";

            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand searchWorkout = new SqlCommand($"select top (1) category from Workout where chatId = '{chatId}'", sql);
                SqlDataReader search = searchWorkout.ExecuteReader();


                while (search.Read())
                {
                    category = (string)search["category"];
                }
                    sql.Close();
            }

            return category;
        }

        public string SelectTranslateFromWorkout(long chatId)
        {
            string translate = "NULL";

            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand searchWorkout = new SqlCommand($"select top (1) translate from Workout where chatId = '{chatId}'", sql);
                SqlDataReader search = searchWorkout.ExecuteReader();


                while (search.Read())
                {
                    translate = (string)search["translate"];
                }
                sql.Close();
            }

            return translate;
        }

        public string SelectWordFromWorkout(long chatId)
        {
            string word = "NULL";

            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand searchWorkout = new SqlCommand($"select top (1) word from Workout where chatId = '{chatId}'", sql);
                SqlDataReader search = searchWorkout.ExecuteReader();


                while (search.Read())
                {
                    word = (string)search["word"];
                }
                sql.Close();
            }

            return word;
        }

        public string SelectRandomEngWordFromWord(long chatId, string category)
        {
            string word = "NULL";

            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение    
                SqlCommand searchWord = new SqlCommand($"select top 1 engWord from Word where chatId = '{chatId}' and Category = N'{category}' ORDER BY NEWID()", sql);
                SqlDataReader sw = searchWord.ExecuteReader();

                while (sw.Read())
                {
                    word = (string)sw["engWord"];

                }
                sql.Close();
            }

            return word;
        }

        public async void AddWordToWorkoutTable(string word, long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand updateWord = new SqlCommand($"update Workout set word = N'{word}' where ChatId = {chatId}", sql);
                await updateWord.ExecuteNonQueryAsync(); //Выполняем команду                                    
                sql.Close();
            }
        }

        public string SelectRandomRusWordFromWord(long chatId, string category)
        {
            string word = "NULL";

            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение    
                SqlCommand searchWord = new SqlCommand($"select top 1 rusWord from Word where chatId = '{chatId}' and Category = N'{category}' ORDER BY NEWID()", sql);
                SqlDataReader sw = searchWord.ExecuteReader();

                while (sw.Read())
                {
                    word = (string)sw["rusWord"];

                }
                sql.Close();
            }

            return word;
        }

        public string SelectRusWordByEngWord(long chatId, string word)
        {
            string rusWord = "NULL";

            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand searchRusWord = new SqlCommand($"select top 1 rusWord from Word where chatId = '{chatId}' and engWord = N'{word}'", sql);
                SqlDataReader sew = searchRusWord.ExecuteReader();

                while (sew.Read())
                {
                    rusWord = (string)sew["rusWord"];

                }
                sql.Close();
            }

            return rusWord;
        }

        public string SelectEngWordByRusWord(long chatId, string word)
        {
            string engWord = "NULL";

            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand searchRusWord = new SqlCommand($"select top 1 engWord from Word where chatId = '{chatId}' and rusWord = N'{word}'", sql);
                SqlDataReader sew = searchRusWord.ExecuteReader();

                while (sew.Read())
                {
                    engWord = (string)sew["engWord"];

                }
                sql.Close();
            }

            return engWord;
        }

        public async void DeleteWorkout(long chatId)
        {
            if (sql.State == ConnectionState.Closed) // Проверяем, закрыто ли sql подключение
            {
                sql.Open(); //Открываем sql подключение
                SqlCommand deleteWorkout = new SqlCommand($"delete from Workout where ChatId = {chatId}", sql);
                await deleteWorkout.ExecuteNonQueryAsync(); //Выполняем команду                                     
                sql.Close();
            }
        }
    }
}
