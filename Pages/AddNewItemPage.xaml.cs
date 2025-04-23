using CollectionManager.Models;
using CollectionManager.Managers;
using System.Collections.ObjectModel;
namespace CollectionManager.Pages;

public partial class AddNewItemPage : ContentPage
{
    private Item _item = new();
    private ObservableCollection<Collection> _collections = new();
    private ObservableCollection<Item> _items = new();
    private string _itemName = String.Empty;
    private string _collectionName = String.Empty;

    public ObservableCollection<string> CollectionsString { get; set; } = new();
    public FileManager FileManager { get; set; } = new();
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            OnPropertyChanged(nameof(Item));
        }
    }

    public string CollectionName
    {
        get => _collectionName;
        set
        {
            _collectionName = value;
            OnPropertyChanged(nameof(CollectionName));
        }
    }

    public ObservableCollection<Collection> Collections
    {
        get => _collections;
        set
        {
            _collections = value;
            OnPropertyChanged(nameof(Collections));
        }
    }
    public ObservableCollection<Item> Items
    {
        get => _items;
        set
        {
            _items = value;
            OnPropertyChanged(nameof(Items));
        }
    }
    private ObservableCollection<string> _itemsString = new();

    public ObservableCollection<string> ItemsString
    {
        get => _itemsString;
        set
        {
            _itemsString = value;
            OnPropertyChanged(nameof(ItemsString));
        }
    }
    public string ItemName
    {
        get => _itemName;
        set
        {
            _itemName = value;
            OnPropertyChanged(nameof(ItemName));
        }
    }

    public AddNewItemPage()
	{
		InitializeComponent();
        Item = new Item();
        Collections = FileManager.LoadCollections();
        CollectionsString = new(FileManager.LoadCollections().Select(c => c.Name).ToList());
        BindingContext = this;
    }

    private bool LookForEmptyFields()
    {
        return
            String.IsNullOrEmpty(Item.Name) ||
            String.IsNullOrEmpty(Item.Comment) ||
            String.IsNullOrEmpty(Item.CollectionName) ||
            String.IsNullOrEmpty(Item.Condition);
    }

    private bool LookForInvalidRating()
    {
        return
            Item.Rating < 1 ||
            Item.Rating > 10;
    }

    private bool LookForInvalidCustomField()
    {
        string[] tab = { Item.CustomFieldName, Item.CustomFieldType, Item.CustomFieldValue };
        List<string> list = new(tab);

        list.RemoveAll(x => String.IsNullOrEmpty(x));

        if (list.Count() == 0 || list.Count() == 3)
        {
            return false;
        }
        return true;
    }

    private void ValidateCustomField()
    {
        switch (Item.CustomFieldType)
        {
            case "Liczba ca�kowita":
                Item.CustomFieldValue = int.Parse(Item.CustomFieldValue).ToString();
                break;

            case "Liczba zmiennoprzecinkowa":
                Item.CustomFieldValue = float.Parse(Item.CustomFieldValue).ToString();
                break;

            default:
                break;
        }
    }

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        if (LookForEmptyFields())
        {
            await DisplayAlert("Error", "Prosz� wype�ni� wszystkie pola", "OK");
            return;
        }

        if (LookForInvalidRating())
        {
            await DisplayAlert("Error", "Ocena przedmiotu powinna si� zawiera� w przedziale <1;10>", "OK");
            return;
        }

        if (Item.Price < 0)
        {
            await DisplayAlert("Error", "Cena przedmiotu  nie mo�e by� ujemna", "OK");
            return;
        }

        if (LookForInvalidCustomField())
        {
            await DisplayAlert("Error", "Wprowadzone dane dla dodatkowego pola s� nie zgodne.", "OK");
            return;
        }

        if (FileManager.CheckIfItemExist(Item))
        {
            bool answer = await DisplayAlert("Ostrze�enie", "Taki przedmiot ju� istnieje.\nCzy chcesz kontynuowa�?", "Tak", "Nie");

            if (!answer)
            {
                await DisplayAlert("Info", "Przedmiot nie zosta� dodany", "OK");
                return;
            }
        }

        ValidateCustomField();

        Item.BgColor = "Crimson";
        Item.Id = FileManager.LoadItems(Item.CollectionName).Count + 1;
        FileManager.AddItem(Item);
        Refresh();
        await DisplayAlert("Info", "Przedmiot zosta� dodany", "OK");

    }

    private void Refresh()
    {
        var navigationStack = Navigation.NavigationStack;
        var currentRoute = Shell.Current?.CurrentState?.Location?.ToString();

        if (MainPage.Instance != null)
        {
            MainPage.Instance.FillItems(Item.CollectionName);
        }
    }

    private async void OnPickImageClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz obraz przedmiotu",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                Item.ImagePath = result.FullPath;
                OnPropertyChanged(nameof(Item));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B��d", $"Nie uda�o si� wczyta� obrazu: {ex.Message}", "OK");
        }
    }
}