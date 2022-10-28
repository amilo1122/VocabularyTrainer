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
        public List<Word>? GetWords(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var sql = @"SELECT * FROM words WHERE categoryid = " + id;
                    return connection.Query<Word>(sql).ToList();
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
    }
}
