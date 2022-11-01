using Dapper;
using Npgsql;
using VocabularyTrainer.Models;

namespace VocabularyTrainer
{
    public class DapperWordRepository
    {
        // Создаем строку подключения к БД
        string connectionString = Config.SQLConnectionString;

        // Добавляем товар в корзину, если отсутствует. Возвращаем false, если товар присутствует или количество <= 0
        public List<Category>? GetCategories()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {                   
                    var sql = @"SELECT id, name FROM categories";
                    return connection.Query<Category>(sql).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        // 
        public List<Word>? GetWords()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM words";
                    return connection.Query<Word>(sql).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        //
        public List<Word>? GetWords(int categoryId)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM words WHERE categoryid = " + categoryId;
                    return connection.Query<Word>(sql).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        //
        public List<String>? GetWords(int typeId, int langId, string columnName)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = "";
                    if (columnName == "To")
                    {
                        sql = @"SELECT toWord FROM words WHERE wordtypeid = " + typeId + "AND toLangId = " + langId;
                    }
                    else
                    {
                        sql = @"SELECT fromWord FROM words WHERE wordtypeid = " + typeId + "AND fromLangId = " + langId;
                    }
                    
                    return connection.Query<string>(sql).ToList();
                }
                catch
                {
                    return null;
                }
            }
        }

        //
        public Word? GetWord(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM words WHERE id = " + id;
                    return connection.QueryFirstOrDefault<Word>(sql);
                }
                catch
                {
                    return null;
                }
            }
        }

        // Возвращаем перевод слова
        public Word TranslateWord(string word)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM words WHERE fromWord = '" + word.ToLower() + "' OR toWord = '" + word.ToLower() + "'";
                    return connection.QueryFirstOrDefault<Word>(sql);
                }
                catch
                {
                    return null;
                }
            }
        }

        // Очищаем текущий прогресс пользователя
        public void ClearCurrectProgress(long id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"DELETE FROM personals WHERE userid = " + id;
                connection.QueryFirstOrDefault<Personal>(sql);
            }
        }

        // Записываем новый прогресс пользователя
        public void SaveNewProgress(long id, List<LearningView> progressList)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                foreach (var item in progressList)
                {
                    var sql = @"INSERT INTO personals(userid, wordid, languageid) VALUES(" + id + "," + item.Id + "," + item.LangId + ");";
                    connection.QueryFirstOrDefault<Personal>(sql);
                }
            }
        }

        // Возвращаем прогресс пользователя
        public List<int> GetUserProgress(long id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"SELECT wordid FROM personals WHERE userid = " + id;
                return connection.Query<int>(sql).ToList();
            }
        }

        // Возвращаем доступные языки
        public List<Language> GetLanguages()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM languages";
                return connection.Query<Language>(sql).ToList();
            }
        }
    }
}
