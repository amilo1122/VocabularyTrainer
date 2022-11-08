using System.Text.Json;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.Menus
{
    public class MainMenu
    {
        List<Menu> mainMenu = new List<Menu> {
            new Menu { Name = "Learn", Callback = "learnWords" },
            new Menu { Name = "Add category", Callback = "newCategory" },
            new Menu { Name = "Add word type", Callback = "newWordType" },
            new Menu { Name = "Add language", Callback = "newLanguage" },
            new Menu { Name = "Add word", Callback = "addWord" },
        };

        public string? GetMainMenu()
        {
            return JsonSerializer.Serialize(mainMenu);
        }
    }
}
