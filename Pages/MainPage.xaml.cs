using CollectionManager.Managers;
using CollectionManager.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CollectionManager.Pages
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {

        public Collection Collection { get; set; }
        public FileManager FileManager { get; set; } = new();
        public ObservableCollection<Collection> Collections { get; set; } = new();
        public ObservableCollection<string> CollectionsString { get; set; } = new();
        public static MainPage Instance { get; set; }

        public ObservableCollection<Item> Items { get; set; } = new();
        private string _collectionString;
        public string CollectionString
        {
            get => _collectionString;
            set
            {
                if (_collectionString != value)
                {
                    _collectionString = value;
                    OnPropertyChanged(nameof(CollectionString));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public MainPage()
        {
            InitializeComponent();
            Collection = new();
            Collections = FileManager.LoadCollections();
            CollectionsString = new(FileManager.LoadCollections().Select(c => c.Name).ToList());
            Instance = this;
            BindingContext = this;
        }

        private bool LookForEmptyFields()
        {
            return String.IsNullOrEmpty(Collection.Name);
        }

        public void FillCollections()
        {
            CollectionsString = new(FileManager.LoadCollections().Select(c => c.Name).ToList());
            OnPropertyChanged(nameof(CollectionsString));
        }

        public void FillItems()
        {
            var loadedItems = FileManager.LoadItems(CollectionString).Where(i => i.Condition != "Sprzedany");
            Items.Clear();
            foreach (var item in loadedItems)
            {
                Items.Add(item);
            }

            var soldItems = FileManager.LoadItems(CollectionString).Where(i => i.Condition == "Sprzedany");

            foreach (var item in soldItems)
            {
                Items.Add(item);
            }
        }


        public void FillItems(string collection)
        {
            var loadedItems = FileManager.LoadItems(collection);

            Items.Clear();
            foreach (var item in loadedItems)
            {
                Items.Add(item);
            }
        }


        private void OnAddCollectionClicked(object sender, EventArgs e)
        {
            if (LookForEmptyFields())
            {
                DisplayAlert("Error", "Proszę wypełnić wszystkie pola", "OK");
                return;
            }

            if (FileManager.CheckIfCollectionExist(Collection.Name))
            {
                DisplayAlert("Error", "Taka kolekcja już istnieje", "OK");
            }
            else
            {
                FileManager.AddCollection(Collection);
                FillCollections();
                Collection.Name = "";
                DisplayAlert("Info", "Kolekcja została dodana", "OK");
            }

        }

        private void OnCollectionChanged(object sender, EventArgs e)
        {
            Debug.WriteLine(CollectionString);
            FillItems();
            OnPropertyChanged(nameof(Items));
        }

        private async void OnImport(object sender, EventArgs e)
        {
            var result = await FilePicker.Default.PickAsync(default);
            if (result != null && result.FileName.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
            {
                FileManager.ImportItems(result.FullPath);
            }
            else
            {
                await DisplayAlert("Import info", "Import failed", "OK");
            }
        }

        private async void OnExport(object sender, EventArgs e)
        {
            if (Items.Count == 0)
            {
                await Shell.Current.DisplayAlert("Export info", "Brak przedmiotów to exportu", "OK");
                return;
            }

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Name,Opis,Cena,Kolekcja,Stan,Ocena,Ścieżka,Id,BgColor");
                foreach (var item in Items)
                {
                    sb.AppendLine($"{item.Name},{item.Comment},{item.Price},{item.CollectionName},{item.Condition},{item.Rating},{item.ImagePath},{item.Id},{item.BgColor}");
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                using var stream = new MemoryStream(bytes);

                var result = await FileSaver.Default.SaveAsync($"{CollectionString}_export.txt", stream, new CancellationToken());

                if (result.IsSuccessful)
                {
                    await Toast.Make($"File saved: {result.FilePath}").Show();
                }
                else
                {
                    await Toast.Make($"Error: {result.Exception?.Message}").Show();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Export error", $"Coś poszło nie tak: {ex.Message}", "OK");
            }
        }

        private void OnSummary(object sender, EventArgs e)
        {
            if (Items.Count() == 0)
            {
                DisplayAlert("Info", "Pusta kolekcja", "OK");
                return;
            }

            List<Item> itemsNotSold = Items.Where(i => i.Condition != "Sprzedany").ToList();
            List<Item> itemsSold = Items.Where(i => i.Condition == "Sprzedany").ToList();
            List<Item> itemsToSold = Items.Where(i => i.Condition == "Chcę sprzedać").ToList();
            StringBuilder sb = new();
            sb.AppendLine($"Ilość posiadanych przedmiotów: {itemsNotSold.Count()}\n");
            sb.AppendLine($"Ilość sprzedanych przedmiotów: {itemsSold.Count()}\n");
            sb.AppendLine($"Ilość przedmiotów do sprzedania: {itemsToSold.Count()}\n");

            DisplayAlert("Podsumowanie", sb.ToString(), "OK");
        }

        private void OnDelete(object sender, EventArgs e)
        {
            Collection.Name = CollectionString;
            FileManager.DeleteCollection(Collection);
            Collections = FileManager.LoadCollections();
            CollectionsString = new(FileManager.LoadCollections().Select(c => c.Name).ToList());
            OnPropertyChanged(nameof(CollectionsString));
            OnPropertyChanged(nameof(Collections));
            collectionPicker.SelectedIndex = -1;
        }
    }

}
