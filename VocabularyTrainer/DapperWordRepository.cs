using Dapper;
using Npgsql;
using VocabularyTrainer.Models;

namespace VocabularyTrainer
{
    public class DapperWordRepository
    {
        // Создаем строку подключения к БД
        string connectionString = Config.SQLConnectionString;

        // Возвращаем все катогории
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

        // Возвращаем все слова
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

        // Возвращем список слов по id категории
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

        // Возвращаем список, сожержащий typeid, langid и имя колонки
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

        // Возвращаем слово по id
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

        // Проверка существования слова в БД
        public bool isWordExists(string fromWord)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM words WHERE fromWord = " + fromWord.ToLower();
                    connection.QueryFirstOrDefault<Word>(sql);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Проверка существования категории в БД
        public bool isCategoryExists(string name)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM category WHERE name = " + name.ToLower();
                    connection.QueryFirstOrDefault<Category>(sql);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Проверка существования типа слова в БД
        public bool isWordTypeExists(string name)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM wordtypes WHERE name = " + name.ToLower();
                    connection.QueryFirstOrDefault<WordType>(sql);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Проверка существования языка в БД
        public bool isLanguageExists(string name)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM languages WHERE name = " + name.ToLower();
                    connection.QueryFirstOrDefault<Language>(sql);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Проверка существования пользователя в БД
        public bool isUserExists(long id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM users WHERE id = " + id;
                    connection.QueryFirstOrDefault<User>(sql);
                    return true;
                }
                catch
                {
                    return false;
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

        // Возвращаем типы слов
        public List<WordType> GetWordTypes()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"SELECT * FROM wordtypes";
                return connection.Query<WordType>(sql).ToList();
            }
        }

        // Сохраняем новое слово в БД
        public void SaveWordToDB(Word word)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO words(fromword, fromlangid, toword, tolangid, wordtypeid, categoryid) 
                            VALUES('" + word.FromWord.ToLower() + "'," + word.FromLangId + ",'" + word.ToWord.ToLower() + "', " 
                            + word.ToLangId + "," + word.WordTypeId + "," + word.CategoryId + ");";
                connection.Query<Word>(sql).ToList();
            }
        }

        // Сохраняем новую категорию в БД
        public void SaveCategoryToDB(string name)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO category(name) 
                            VALUES('" + name.ToLower() + "');";
                connection.Query<Category>(sql).ToList();
            }
        }

        // Сохраняем новый тип слова в БД
        public void SaveWordTypeToDB(string name)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO wordtypes(name) 
                            VALUES('" + name.ToLower() + "');";
                connection.Query<WordType>(sql).ToList();
            }
        }

        // Сохраняем новый язык в БД
        public void SaveLanguageToDB(string name)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO languages(name) 
                            VALUES('" + name.ToLower() + "');";
                connection.Query<Language>(sql).ToList();
            }
        }

        // Сохраняем нового пользователя в БД
        public void SaveUserToDB(long id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO users(id) 
                            VALUES(" + id + ");";
                connection.Query<Language>(sql).ToList();
            }
        }
    }
}
