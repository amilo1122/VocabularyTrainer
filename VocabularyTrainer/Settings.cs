
using System.Text.RegularExpressions;
using VocabularyTrainer.Menus;
using VocabularyTrainer.Models;

namespace VocabularyTrainer
{
    public class Settings
    {
        DapperWordRepository repo = new DapperWordRepository();

        Dictionary<long, NewWordView> newWordDict = new Dictionary<long, NewWordView>();
        Dictionary<long, List<string>> learningDict = new Dictionary<long, List<string>>();
        public string? LoadMainMenu(long id)
        {
            MainMenu mainMenu = new MainMenu();
            var userMenu = mainMenu.GetMainMenu();
            return userMenu;
        }

        public string? LoadLearningMenu(long id)
        {
            LearningMenu learningMenu = new LearningMenu();
            var userMenu = learningMenu.GetLearningMenu();
            return userMenu;
        }

        public bool AddEnglishWord(string name, long userId)
        {
            if (Regex.IsMatch(name, "[a-zA-Z]"))
            {
                var newWord = new NewWordView();
                newWord.EngWord = name;
                if (newWordDict.ContainsKey(userId))
                {
                    newWordDict.Remove(userId);
                }
                newWordDict.Add(userId, newWord);
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Category>? GetCategories()
        {
            return repo.GetCategories();
        }

        public void GenerateWords(long userId, string category)
        {
            var categoryId = Int32.Parse(category.Replace("setCategories", ""));
            var words = new List<Word>();
            if (categoryId != 0)
            {
                words = repo.GetWords(categoryId);
            }
            else
            {
                words = repo.GetWords();
            }
            var engList = new List<string>();
            var rusList = new List<string>();
            foreach(var word in words)
            {
                engList.Add(word.EngWord);
                rusList.Add(word.RusWord);
            }
            if (learningDict.ContainsKey(userId))
            {
                learningDict.Remove(userId);
            }
            learningDict[userId] = engList.Concat(rusList).ToList();
        }
    }
}
