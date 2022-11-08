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

using System.Net;

ServicePointManager.Expect100Continue = true;
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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
Dictionary<long, LearningView> _currentWordDict = new Dictionary<long, LearningView>();
// Объявляем словарь для хранения нового слова
Dictionary<long, Word> _newWordDict = new Dictionary<long, Word>();

// Объявляем массив методов для обработки сообщений от пользователей
List<Method> methods = new List<Method>()
{
    new Method(1, MisundestatingAnswer),
    new Method(2, CheckTranslation),
    new Method(3, SetFromWord),
    new Method(4, SetToWord),
    new Method(5, AddCategory),
    new Method(6, AddWordType),
    new Method(7, AddLanguage),
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

Console.WriteLine($"Started wiretapping @{me.Username}");
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
            await CheckUser(botClient, message);
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
            await StartLearning(botClient, callbackQuery.Message);
            break; 
        case string s when s.StartsWith("getNext"):
            await StartLearning(botClient, callbackQuery.Message);
            break; 
        case string s when s.StartsWith("enterTranslation"):
            await AskTranslation(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("showTranslation"):
            await ShowTranslation(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("runTest"):
            await DisplayTest(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("testWord-"):
            var message = callbackQuery.Message;
            message.Text = callbackQuery.Data.Replace("testWord-","");
            await CheckTranslation(botClient, message);
            break;
        case string s when s.StartsWith("saveProgress"):
            await SaveProgress(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("uploadProgress"):
            await LoadProgress(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("addWord"):
            await DisplayLanguage(botClient, callbackQuery.Message);
            break;
        case string s when s.StartsWith("FromLang-"):
            await AskFromWord(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("ToLang-"):
            await AskToWord(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("wordTypes"):
            await SetWordType(botClient, callbackQuery);
            await DisplayCategories(botClient, callbackQuery, true);
            break; 
        case string s when s.StartsWith("writeWordToDB"):
            await SaveWordToDB(botClient, callbackQuery);
            break; 
        case string s when s.StartsWith("newCategory"):
            await AskNewCategoryName(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("newWordType"):
            await AskNewWordTypeName(botClient, callbackQuery);
            break;
        case string s when s.StartsWith("newLanguage"):
            await AskNewLanguageName(botClient, callbackQuery);
            break;
    }
}

async Task CheckUser(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    settings.CheckUser(id);
    await botClient.SendTextMessageAsync(id, $"Hello, {message.Chat.Username}!, welcome to FFR Language Trainer:");
}

async Task AskNewLanguageName(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter new language name:");
    State = 7;
}

async Task AskNewWordTypeName(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter new word type name:");
    State = 6;
}

async Task AskNewCategoryName(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter new category name:");
    State = 5;
}

async Task SaveWordToDB(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    var category = Int32.Parse(callbackQuery.Data.Replace("writeWordToDB",""));
    var word = _newWordDict[id];
    word.CategoryId = category;
    _newWordDict[id] = word;

    var flag = settings.SaveWordToDB(_newWordDict[id]);
    if (flag)
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "The word has been successfully added to datebase");
    }
    else
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "The word has alredy been in datebase");
    }
    _newWordDict.Remove(id);
    await LoadMainMenu(botClient, callbackQuery.Message);
}

async Task SetWordType(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    var chosenWordType = callbackQuery.Data.Replace("wordTypes", "");
    var word = _newWordDict[id];
    word.WordTypeId = Int32.Parse(chosenWordType);
    _newWordDict[id] = word;
}

async Task AskToWord(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    var chosenLanguage = callbackQuery.Data.Replace("ToLang-", "");
    var word = _newWordDict[id];
    word.ToLangId = Int32.Parse(chosenLanguage);
    _newWordDict[id] = word;
    await botClient.SendTextMessageAsync(id, "Enter to word:");
    State = 4;
}

async Task AskFromWord(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    var chosenLanguage = callbackQuery.Data.Replace("FromLang-", "");
    if (_newWordDict.ContainsKey(id))
    {
        _newWordDict.Remove(id);
    }
    Word word = new Word();
    word.FromLangId = Int32.Parse(chosenLanguage);
    _newWordDict[id] = word;

    await botClient.SendTextMessageAsync(id, "Enter from word:");
    State = 3;
}

async Task DisplayLanguage(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    
    var langs = settings.GetLanguages();

    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

    string outputString = "";

    if (langs != null)
    {
        foreach (var lang in langs)
        {
            if (!_newWordDict.ContainsKey(id))
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{lang.Name}", callbackData: "FromLang-" + lang.Id));
                outputString = "From language:";
            }
            else
            {
                if (_newWordDict[id].FromLangId != lang.Id)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{lang.Name}", callbackData: "ToLang-" + lang.Id));
                }
                outputString = "To language:";
            }
            
        }
        InlineKeyboardMarkup newWordKeyboard = new InlineKeyboardMarkup(SetOneColumnMenu(buttons).ToArray());
        await botClient.SendTextMessageAsync(id, outputString, replyMarkup: newWordKeyboard);
    }
    else
    {
        await botClient.SendTextMessageAsync(id, "The list is empty");
    }
}

async Task SaveProgress(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    settings.SaveProgress(id);
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Currect progress has been saved to database");
    await LoadMainMenu(botClient, callbackQuery.Message);
}

async Task DisplayTest(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    var currentWord = _currentWordDict[id];
    var testList = settings.GetTestList(id, currentWord);

    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();

    if (testList != null)
    {
        foreach (var word in testList)
        {
            buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{word}", callbackData: "testWord-" + word));
        }
        InlineKeyboardMarkup testWordsKeyboard = new InlineKeyboardMarkup(SetOneColumnMenu(buttons).ToArray());
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Choose the correct translation of the word: {currentWord.Name}", replyMarkup: testWordsKeyboard);
    }
    else
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "The list is empty");
    }

}

async Task ShowTranslation(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    var currentWord = _currentWordDict[id];
    var translation = settings.TranslateWord(currentWord.Name);
    await botClient.SendTextMessageAsync(
            chatId: id,
            text: $"<b>{currentWord.Name}</b> translates like <b>{translation}</b>",
            parseMode: ParseMode.Html,
            disableNotification: true);
    await StartLearning(botClient, callbackQuery.Message);
}

async Task AskTranslation(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Enter the translation of the word:");
    State = 2;
}

async Task StartLearning(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    
    if(!settings.isCollectionEmpty(id))
    {
        var nextWord = settings.GetNextWord(id);
        Console.WriteLine($"Next word is {nextWord.Name}");
        if (_currentWordDict.ContainsKey(id))
        {
            _currentWordDict.Remove(id);
        }
        _currentWordDict[id] = nextWord;
        await botClient.SendTextMessageAsync(
            chatId: id,
            text: $"<b>{nextWord.Name}</b>",
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
        await botClient.SendTextMessageAsync(id, "Well done, all words has been learnt");
        await LoadMainMenu(botClient, message);
    }    
}

async Task LoadProgress(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    var id = callbackQuery.Message.Chat.Id;
    settings.LoadProgress(id);
    await botClient.SendTextMessageAsync(id, "The progress has been loaded");
    await StartLearning(botClient, callbackQuery.Message);
}

async Task DisplayCategories(ITelegramBotClient botClient, CallbackQuery callbackQuery, bool isNewWord = false)
{
    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
    var categories = settings.GetCategories();

    if (categories != null)
    {
        foreach (var category in categories)
        {
            if (!isNewWord)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{category.Name}", callbackData: "setCategories" + category.Id));
            }
            else
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"{category.Name}", callbackData: "writeWordToDB" + category.Id));
            }
            
        }
        if (!isNewWord)
        {
            buttons.Add(InlineKeyboardButton.WithCallbackData(text: $"All categories", callbackData: "setCategories0"));
        }        
        InlineKeyboardMarkup goodsKeyboard = new InlineKeyboardMarkup(SetOneColumnMenu(buttons).ToArray());
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Choose a category:", replyMarkup: goodsKeyboard);
    }
    else
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "The list of categories is empty!");
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

    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Main menu", replyMarkup: mainLearningKeyboard);

}

// Преверка введеного перевода. Если перевод неверный - слово оставляем в сгенерированный коллекции
async Task CheckTranslation(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    var currentWord = _currentWordDict[id];
    var translation = settings.TranslateWord(currentWord.Name);
    if (translation != null)
    {
        if (translation == message.Text.ToLower())
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, text: "Correct!");
            settings.DeleteLearningWord(id, currentWord.Name);
            Console.WriteLine($"{currentWord.Name} was deleted from learning collection");
            await StartLearning(botClient, message);
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, text: $"You're wrong! It was - {translation}");
            Console.WriteLine($"{currentWord.Name} was NOT deleted from learning collection");
            await StartLearning(botClient, message);
        }
    }
    else
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, text: "Warning! Word is unavailable in database");
    }    
}

// Записываем fromWord в словарь и выводим список языков
async Task SetFromWord(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    var fromWord = message.Text;
    var word = _newWordDict[id];
    word.FromWord = fromWord;
    _newWordDict[id] = word;
    
    await DisplayLanguage(botClient, message);
}

// Записываем toWord в словарь и выводим список типов
async Task SetToWord(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    var toWord = message.Text;
    var word = _newWordDict[id];
    word.ToWord = toWord;
    _newWordDict[id] = word;

    await DisplayWordTypes(botClient, message);
}

// Записываем новую категорию в БД
async Task AddCategory(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    var name = message.Text;
    bool flag = settings.AddCategory(name);
    if (flag)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Category has been successully added");
    }
    else
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Category already exists in datebase");
    }

    await LoadMainMenu(botClient, message);
}

// Записываем новый тип слова в БД
async Task AddWordType(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    var name = message.Text;
    bool flag = settings.AddWordType(name);
    if (flag)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Word type has been successully added");
    }
    else
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Word type already exists in datebase");
    }

    await LoadMainMenu(botClient, message);
}

// Записываем новый тип слова в БД
async Task AddLanguage(ITelegramBotClient botClient, Message message)
{
    var id = message.Chat.Id;
    var name = message.Text;
    bool flag = settings.AddLanguage(name);
    if (flag)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Language has been successully added");
    }
    else
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Language already exists in datebase");
    }

    await LoadMainMenu(botClient, message);
}

// Выводим типы слов
async Task DisplayWordTypes(ITelegramBotClient botClient, Message message)
{
    var types = settings.GetWordTypes();
    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
    
    foreach (var button in types)
    {
        buttons.Add(InlineKeyboardButton.WithCallbackData(text: button.Name, callbackData: "wordTypes" + button.Id));
    }

    InlineKeyboardMarkup typesKeyboard = new InlineKeyboardMarkup(SetTwoColumnsMenu(buttons).ToArray());

    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose word type", replyMarkup: typesKeyboard);
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

    await botClient.SendTextMessageAsync(message.Chat.Id, "Main menu", replyMarkup: mainMenuKeyboard);
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
    await botClient.SendTextMessageAsync(message.Chat.Id, text: "I don't understand this command...");

    await LoadMainMenu(botClient, message);
}

// Обработка ошибок
Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API error:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

