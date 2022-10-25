using System.Text.Json;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.Menus
{
    public class MainMenu
    {
        List<Menu> mainMenu = new List<Menu> {
            new Menu { Name = "Learn", Callback = "learnWords" },
            new Menu { Name = "Add new", Callback = "addWord" },
            new Menu { Name = "Translate", Callback = "translateWord" },
        };

        public string? GetMainMenu()
        {
            return JsonSerializer.Serialize(mainMenu);
        }
    }
}
