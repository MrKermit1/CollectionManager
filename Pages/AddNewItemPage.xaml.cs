using CollectionManager.Models;
using CollectionManager.Managers;
using System.Collections.ObjectModel;
namespace CollectionManager.Pages;

public partial class AddNewItemPage : ContentPage
{
	public Item Item { get; set; }
    public FileManager FileManager { get; set; } = new();
    public ObservableCollection<Collection> Collections { get; set; } = new();
    public ObservableCollection<string> CollectionsString { get; set; } = new();

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
            case "Liczba ca³kowita":
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
            await DisplayAlert("Error", "Proszê wype³niæ wszystkie pola", "OK");
            return;
        }

        if (LookForInvalidRating())
        {
            await DisplayAlert("Error", "Ocena przedmiotu powinna siê zawieraæ w przedziale <1;10>", "OK");
            return;
        }

        if (Item.Price < 0)
        {
            await DisplayAlert("Error", "Cena przedmiotu  nie mo¿e byæ ujemna", "OK");
            return;
        }

        if (LookForInvalidCustomField())
        {
            await DisplayAlert("Error", "Wprowadzone dane dla dodatkowego pola s¹ nie zgodne.", "OK");
            return;
        }

        if (FileManager.CheckIfItemExist(Item))
        {
            bool answer = await DisplayAlert("Ostrze¿enie", "Taki przedmiot ju¿ istnieje.\nCzy chcesz kontynuowaæ?", "Tak", "Nie");

            if (!answer)
            {
                await DisplayAlert("Info", "Przedmiot nie zosta³ dodany", "OK");
                return;
            }
        }

        ValidateCustomField();

        Item.BgColor = "Crimson";
        Item.Id = FileManager.LoadItems(Item.CollectionName).Count + 1;
        FileManager.AddItem(Item);
        Refresh();
        await DisplayAlert("Info", "Przedmiot zosta³ dodany", "OK");

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

    private void OnSwitch(object sender, ToggledEventArgs e)
    {

    }
}