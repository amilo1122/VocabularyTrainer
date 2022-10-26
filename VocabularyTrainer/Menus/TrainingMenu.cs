using System.Text.Json;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.Menus
{
    public class TrainingMenu
    {
        List<Menu> trainingMenu = new List<Menu> {
            new Menu { Name = "Enter translation", Callback = "enterTranslation" },
            new Menu { Name = "Run test", Callback = "runTest" },
            new Menu { Name = "Show translation", Callback = "showTranslation" },
            new Menu { Name = "Add to personal list", Callback = "addPersonal" },
            new Menu { Name = "Save progress", Callback = "saveProgress" },
            new Menu { Name = "Get next", Callback = "getNext" },
        };

        public string? GetTrainingMenu()
        {
            return JsonSerializer.Serialize(trainingMenu);
        }
    }
}
