
using System.Text.RegularExpressions;
using VocabularyTrainer.Menus;
using VocabularyTrainer.Models;

namespace VocabularyTrainer
{
    public class Settings
    {
        DapperWordRepository repo = new DapperWordRepository();
        Random rnd = new Random();

        Dictionary<long, List<LearningView>> learningDict = new Dictionary<long, List<LearningView>>();
        public string? LoadMainMenu()
        {
            MainMenu mainMenu = new MainMenu();
            var userMenu = mainMenu.GetMainMenu();
            return userMenu;
        }

        public string? LoadLearningMenu()
        {
            LearningMenu learningMenu = new LearningMenu();
            var userMenu = learningMenu.GetLearningMenu();
            return userMenu;
        }

        public string? LoadTrainingMenu()
        {
            TrainingMenu trainingMenu = new TrainingMenu();
            var userMenu = trainingMenu.GetTrainingMenu();
            return userMenu;
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
            PreparingLearningList(userId, words);
        }

        private void PreparingLearningList(long userId, List<Word> words)
        {
            var engList = new List<LearningView>();

            var rusList = new List<LearningView>();

            foreach (var word in words)
            {
                var engObj = new LearningView();
                engObj.Id = word.Id;
                engObj.Name = word.FromWord;
                engObj.LangId = word.FromLangId;
                engList.Add(engObj);
                var rusObj = new LearningView();
                rusObj.Id = word.Id;
                rusObj.Name = word.ToWord;
                rusObj.LangId = word.ToLangId;
                rusList.Add(rusObj);
            }
            if (learningDict.ContainsKey(userId))
            {
                learningDict.Remove(userId);
            }
            learningDict[userId] = engList.Concat(rusList).ToList();
        }

        public bool isCollectionEmpty(long id)
        {
            var userList = learningDict[id];
            if (userList.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public LearningView GetNextWord(long id)
        {
            var userList = learningDict[id];
            var index = rnd.Next(0, userList.Count);
            return userList[index];
        }

        public void DeleteLearningWord(long id, string word)
        {
            var userList = learningDict[id];
            int index = 0;
            for (int i = 0; i < userList.Count; i++)
            {
                if (userList[i].Name == word)
                {
                    index = i;
                }
            }
            userList.RemoveAt(index);
            learningDict.Remove(id);
            learningDict[id] = userList;
        }

        public string? TranslateWord(string fromWord)
        {
            var word = repo.TranslateWord(fromWord);
            if (word != null)
            {
                if (word.FromWord == fromWord)
                {
                    return word.ToWord;
                }
                else
                {
                    return word.FromWord;
                }
            }
            return null;
        }

        public List<string> GetTestList(long id, LearningView currentWord)
        {
            var translation = TranslateWord(currentWord.Name);
            var targetWord = repo.GetWord(currentWord.Id);
            var typeId = targetWord.WordTypeId;
            int targetLangId = -1;
            var columnName = "";
            if (targetWord.ToLangId != currentWord.LangId)
            {
                targetLangId = targetWord.ToLangId;
                columnName = "To";
            }
            else
            {
                targetLangId = targetWord.FromLangId;
                columnName = "From";
            }
            var targetList = repo.GetWords(typeId, targetLangId, columnName);
            if (targetList != null && translation != null)
            {
                targetList.Remove(translation);

                var outputList = new List<string>();
                int index = 3;
                if (targetList.Count < 3)
                {
                    index = targetList.Count;
                }
                int[] indexArray = new int[index];
                for (int i = 0; i < indexArray.Length; i++)
                {
                    indexArray[i] = -1;
                }
                int j = 0;
                while (indexArray.Contains(-1))
                {
                    var targetIndex = rnd.Next(0, targetList.Count);
                    if (!indexArray.Contains(targetIndex))
                    {
                        indexArray[j] = targetIndex;
                        outputList.Add(targetList[targetIndex]);
                        j++;
                    }
                }
                outputList.Add(translation);
                outputList.Sort();
                return outputList;
            }
            else
            {
                return null;
            }            
        }

        // 
        public void SaveProgress(long id)
        {
            repo.ClearCurrectProgress(id);
            var newProgess = learningDict[id];
            repo.SaveNewProgress(id, newProgess);
        }

        public void LoadProgress(long id)
        {
            var userProgress = repo.GetUserProgress(id);
            List<Word> result = new List<Word>();
            foreach (var wordId in userProgress)
            {
                var word = repo.GetWord(wordId);
                result.Add(word);
            }
            PreparingLearningList(id, result);
        }

        public List<Language> GetLanguages()
        {
            return repo.GetLanguages();
        }

        public List<WordType> GetWordTypes()
        {
            return repo.GetWordTypes();
        }

        public bool SaveWordToDB(Word word)
        {
            bool isExists = repo.isWordExists(word.FromWord);
            if (!isExists)
            {
                repo.SaveWordToDB(word);
                return true;
            }
            return false;
        }
    }
}
