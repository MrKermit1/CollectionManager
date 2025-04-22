using CollectionManager.Models;
using CollectionManager.Managers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CollectionManager.Pages;

public partial class EditItemPage : ContentPage, INotifyPropertyChanged
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



    public EditItemPage()
	{
		InitializeComponent();
        Collections = FileManager.LoadCollections();
        CollectionsString = new(FileManager.LoadCollections().Select(c => c.Name).ToList());
        Item = new();

        BindingContext = this;
    }

    private bool LookForEmptyFields()
    {
        return
            String.IsNullOrEmpty(Item.Name) ||
            String.IsNullOrEmpty(Item.Comment) ||
            String.IsNullOrEmpty(Item.CollectionName) ||
            String.IsNullOrEmpty(Item.Condition) ||
            String.IsNullOrEmpty(ItemName);
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
            case "Liczba ca³kowita":
                if (int.TryParse(Item.CustomFieldValue, out int intValue))
                {
                    Item.CustomFieldValue = intValue.ToString();
                }
                else
                {
                    Item.CustomFieldValue = "0";
                }
                break;

            case "Liczba zmiennoprzecinkowa":
                if (float.TryParse(Item.CustomFieldValue, out float floatValue))
                {
                    Item.CustomFieldValue = floatValue.ToString();
                }
                else
                {
                    Item.CustomFieldValue = "0.0";
                }
                break;

            default:
                break;
        }
    }

    private void SaveChanges(object sender, EventArgs e)
    {
        if (LookForEmptyFields())
        {
            DisplayAlert("Error", "Proszê wype³niæ wszystkie pola", "OK");
            return;
        }

        if (LookForInvalidRating())
        {
            DisplayAlert("Error", "Ocena przedmiotu powinna siê zawieraæ w przedziale <1;10>", "OK");
            return;
        }

        if (Item.Price < 0)
        {
            DisplayAlert("Error", "Cena przedmiotu  nie mo¿e byæ ujemna", "OK");
            return;
        }

        if (LookForInvalidCustomField())
        {
            DisplayAlert("Error", "Wprowadzone dane dla dodatkowego pola s¹ nie zgodne.", "OK");
            return;
        }

        ValidateCustomField();
        FileManager.UpdateItem(Item);
        Refresh();
        DisplayAlert("Info", "Przedmiot zosta³ dodany", "OK");
    }

    private void SelectedCollectionChanged(object sender, EventArgs e)
    {
        Items.Clear();
        Items = FileManager.LoadItems(CollectionName);
        ItemsString = new(Items.Select(i => i.Name));
        
        itemPicker.SelectedIndex = -1;
    }

    private void SelectedItemChanged(object sender, EventArgs e)
    {        
        Item = Items.FirstOrDefault(i => i.Name == ItemName);
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
            await DisplayAlert("B³¹d", $"Nie uda³o siê wczytaæ obrazu: {ex.Message}", "OK");
        }
    }
}