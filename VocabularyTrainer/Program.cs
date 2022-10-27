using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using VocabularyTrainer;
using VocabularyTrainer.Models;

// Объявляем переменную для хранения ключа
const string key = @"c:/apikey/apikey1.txt";
// Читаем ключ из файла
var apiString = ReadApiKey(key);

var botClient = new TelegramBotClient(apiString);
Settings settings = new Settings();

// Объявляем токен отмены
using var cts = new CancellationTokenSource();
// Объявляем переменную статуса
var State = 0;
// Объявляем словарь для обработки слова
Dictionary<long, string> _currentWordDict = new Dictionary<long, string>();

// Объявляем массив методов для обработки сообщений от пользователей
List<Method> methods = new List<Method>()
{
    new Method(1, MisundestatingAnswer),
    new Method(2, CheckTranslation),
};

// Объявляем настройки получения обновлений
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

// Компануем и начинаем получать обновления от телеграмма
botClient.StartReceiving(
    //название метода, в котором обрабатываем обновление бота
    HandleUpdatesAsync,
    //название метода, в котором обработываем обшибки
    HandleErrorAsync,
    //настройки получений обновлений
    receiverOptions,
    //токен отмены
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Начал прослушку @{me.Username}");
Console.ReadLine();

// Указываем токен отмены
cts.Cancel();

// Читаем API ключ
string ReadApiKey(string path)
{
    return System.IO.File.ReadAllText(path);
}

// Обрабатываем Update
async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    try
    {
        switch (update.Type)
        {
            case (UpdateType.Message):
                await BotOnMessageReceived(botClient, update.Message);
                break;
            case (UpdateType.CallbackQuery):
                await InlineModeProcessing(botClient, update.CallbackQuery);
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

async Task BotOnMessageReceived(ITelegramBotClient botClient, Message? message)
{
    Console.WriteLine($"Receive message type {message.Type}");
    if (message.Type != MessageType.Text)
    {
        return;
    }
    switch (message.Text)
    {
        case ("/start"):
            await LoadMainMenu(botClient, message);
            break;
        default:
            await OnMessageProcessing(botClient, message);
            break;
    }
}
async Task InlineModeProcessing(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    Console.WriteLine($"Receive message type {callbackQuery.Message.Type}");

    switch (callbackQuery.Data)
    {
        case string s when s.StartsWith("mainMenu"):
            await LoadMainMenu(botClient, callbackQuery.Message);
            break;
        case string s when s.StartsWith("learnWords"):
            await DisplayLearningMenu(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("chooseCategory"):
            await DisplayCategories(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("uploadProgress"):
            await LoadProgress(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("setCategories"):
            settings.GenerateWords(callbackQuery.Message.Chat.Id, callbackQuery.Data);
            await StartLearning(botClient, callbackQuery);
            break; 
        case string s when s.StartsWith("getNext"):
            await StartLearning(botClient, callbackQuery);
            break; 
        case string s when s.StartsWith("enterTranslation"):
            await AskTranslation(botClient, callbackQuery);
            break;
    }
}

async Task AskTranslation(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Введите перевод слова:");
    State = 2;
}

async Task StartLearning(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    
    if(!settings.isCollectionEmpty(id))
    {
        var nextWord = settings.GetNextWord(id);
        if (_currentWordDict.ContainsKey(id))
        {
            _currentWordDict.Remove(id);
        }
        _currentWordDict[id] = nextWord;
        await botClient.SendTextMessageAsync(
            chatId: id,
            text: $"<b>{nextWord}</b>",
            parseMode: ParseMode.Html,
            disableNotification: true);
        List<Menu> trainingMenu = JsonSerializer.Deserialize<List<Menu>>(settings.LoadTrainingMenu());
        List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

        foreach (var button in trainingMenu)
        {
            buttons.Add(InlineKeyboardButton.WithCallbackData(text: button.Name, callbackData: button.Callback));
        }

        InlineKeyboardMarkup trainingKeyboard = new InlineKeyboardMarkup(SetTwoColumnsMenu(buttons).ToArray());

        await botClient.SendTextMessageAsync(id, "Allowed actions:", replyMarkup: trainingKeyboard);
    }
    else
    {
        await botClient.SendTextMessageAsync(id, "Все слова из выбранной категории проработаны");
        await LoadMainMenu(botClient, message);
    }    
}

async Task LoadProgress(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    throw new NotImplementedException();
}

async Task DisplayCategories(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
    var categories = settings.GetCategories();

    if (categories != null)
    {
        foreach (var category in categories)
        {
            buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{category.Name}", callbackData: "setCategories" + category.Id));
        }
        buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"Все категории", callbackData: "setCategories0"));
        InlineKeyboardMarkup goodsKeyboard = new InlineKeyboardMarkup(SetOneColumnMenu(buttons).ToArray());
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Выберите категорию для обучения:", replyMarkup: goodsKeyboard);
    }
    else
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Список категорий пуст!");
    }
}

async Task DisplayLearningMenu(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    List<Menu> learningMenu = JsonSerializer.Deserialize<List<Menu>>(settings.LoadLearningMenu());
    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

    foreach (var button in learningMenu)
    {
        buttons.Add(InlineKeyboardButton.WithCallbackData(text: button.Name, callbackData: button.Callback));
    }

    InlineKeyboardMarkup mainLearningKeyboard = new InlineKeyboardMarkup(SetTwoColumnsMenu(buttons).ToArray());

    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Главное меню", replyMarkup: mainLearningKeyboard);

}

// 
async Task CheckTranslation(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    var currentWord = _currentWordDict[id];
    var translation = settings.TranslateWord(currentWord);
    if (translation == currentWord)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, text: "Все верно!");
        await StartLearning(botClient, message);
    }
    else
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, text: $"Ошибка! Правильный перевод - {translation}");
    }
    

    await LoadMainMenu(botClient, message);
}

// Обрабатываем запросы типа Message
async Task OnMessageProcessing(ITelegramBotClient botClient, Message message)
{
    Method method = methods.FirstOrDefault(x => x.State == State);
    if (method != null)
    {
        method.DelegateMethod(botClient, message);
    }
}

// Выводим кнопки главного меню
async Task LoadMainMenu(ITelegramBotClient botClient, Message message)
{
    List<Menu> mainMenu = JsonSerializer.Deserialize<List<Menu>>(settings.LoadMainMenu());
    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

    foreach (var button in mainMenu)
    {
        buttons.Add(InlineKeyboardButton.WithCallbackData(text: button.Name, callbackData: button.Callback));
    }

    InlineKeyboardMarkup mainMenuKeyboard = new InlineKeyboardMarkup(SetTwoColumnsMenu(buttons).ToArray());

    await botClient.SendTextMessageAsync(message.Chat.Id, "Главное меню", replyMarkup: mainMenuKeyboard);
}

// Выводим кнопки в две колонки
List<InlineKeyboardButton[]> SetTwoColumnsMenu(List<InlineKeyboardButton> buttons)
{
    List<InlineKeyboardButton[]> twoColumnsMenu = new List<InlineKeyboardButton[]>();
    for (var i = 0; i < buttons.Count; i++)
    {
        if (buttons.Count - 1 == i)
        {
            twoColumnsMenu.Add(new[] { buttons[i] });
        }
        else
            twoColumnsMenu.Add(new[] { buttons[i], buttons[i + 1] });
        i++;
    }
    return twoColumnsMenu;
}

// Выводим кнопки в одну колонку
List<InlineKeyboardButton[]> SetOneColumnMenu(List<InlineKeyboardButton> buttons)
{
    List<InlineKeyboardButton[]> oneColumnMenu = new List<InlineKeyboardButton[]>();
    for (var i = 0; i < buttons.Count; i++)
    {
        oneColumnMenu.Add(new[] { buttons[i] });
    }
    return oneColumnMenu;
}

// Выводим сообщение о непонятной команде, когда пользователь пишет сообщение и State=0, далее выводим главное меню
async Task MisundestatingAnswer(ITelegramBotClient botClient, Message message)
{
    await botClient.SendTextMessageAsync(message.Chat.Id, text: "Я не понимаю такой команды...");

    await LoadMainMenu(botClient, message);
}

// Обработка ошибок
Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Ошибка телеграм API:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

