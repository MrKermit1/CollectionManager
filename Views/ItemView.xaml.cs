using CollectionManager.Managers;
using CollectionManager.Models;
using CollectionManager.Pages;

namespace CollectionManager.Views;

public partial class ItemView : ContentView
{

    public static readonly BindableProperty ItemProperty =
        BindableProperty.Create(
            nameof(Item),
            typeof(Item),
            typeof(ItemView),
            default(Item),
            BindingMode.TwoWay,
            propertyChanged: OnProductModelChanged
        );

    public Item Item
    {
        get => (Item)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public ItemView()
	{
		InitializeComponent();
	}

    public ItemView(Item item)
    {
        InitializeComponent();
        Item = item;
        BindingContext = this;
    }

    private static void OnProductModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ItemView itemView)
        {
            itemView.BindingContext = newValue;
        }
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

    private void OnSaleClicked(object sender, EventArgs e)
    {
        FileManager fm = new();
        Item.Condition = "Sprzedany";
        Item.BgColor = "DimGrey";
        fm.UpdateItem(Item);
        Refresh();
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        FileManager fm = new();
        fm.DeleteItem(Item);
        Refresh();
    }
}