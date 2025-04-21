using CollectionManager.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManager.Managers
{

    public class FileManager
    {
        private static readonly string collectionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "collections.txt");
        private static readonly string imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
        private string[] itemHeaders = { "Name", "Opis", "Cena", "Kolekcja", "Stan", "Ocena", "Ścieżka", "Id", "BgColor", "CustomNazwa", "CustomTyp", "CustomWartość" };
        private void CreateTextFile(string path)
        {
            File.Create(path).Dispose();
        }

        public bool CheckIfCollectionExist(string collectionName)
        {
            if (File.Exists(collectionsPath))
            {
                return LoadCollections().Any(c => c.Name == collectionName);
            }
            return false;
        }

        public bool CheckIfItemExist(Item item)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{item.CollectionName}.txt");
            if (File.Exists(filePath))
            {
                return LoadItems(item.CollectionName).Any(i => i.Name == item.Name);
            }

            return false;
        }

        public ObservableCollection<Item> LoadItems(string collectionName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{collectionName}.txt");
            if (File.Exists(filePath))
            {
                List<Item> items = File.ReadAllLines(filePath)
                             .Skip(1)
                             .Select(csv => Item.FromCsv(csv))
                             .ToList();
                return new ObservableCollection<Item>(items);
            }

            return new ObservableCollection<Item>();
        }

        public ObservableCollection<Collection> LoadCollections()
        {
            if (File.Exists(collectionsPath))
            {
                List<Collection> collections = File.ReadAllLines(collectionsPath)
                             .Skip(1)
                             .Select(csv => Collection.FromCsv(csv))
                             .ToList();
                return new ObservableCollection<Collection>(collections);
            }

            return new ObservableCollection<Collection>();
        }

        public void AddCollection(Collection collection)
        {
            if (!File.Exists(collectionsPath))
            {
                CreateTextFile(collectionsPath);
            }
            StringBuilder sb = new StringBuilder();

            List<Collection> collections = LoadCollections().ToList();
            sb.AppendLine("Name");

            collections.Add(collection);

            foreach (var item in collections)
            {
                sb.AppendLine($"{item.Name}");
            }

            try
            {
                File.WriteAllText(collectionsPath, String.Empty);
                File.AppendAllText(collectionsPath, sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }

            Debug.WriteLine("\nZapis kolekcji udany\n");
        }

        public void AddItem(Item item)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{item.CollectionName}.txt");
            if (!File.Exists(filePath))
            {
                CreateTextFile(filePath);
            }

            StringBuilder sb = new StringBuilder();

            List<Item> items = LoadItems(item.CollectionName).ToList();

            sb.AppendLine(string.Join(",", itemHeaders));

            if (!String.IsNullOrEmpty(item.ImagePath))
            {
                string tempPath = item.ImagePath;

                item.ImagePath = Path.Combine(imagesPath, Path.GetFileName(item.ImagePath));
                if (!File.Exists(item.ImagePath))
                {
                    File.Copy(tempPath, Path.Combine(imagesPath, Path.GetFileName(tempPath)));
                }
            }

            items.Add(item);

            foreach (var i in items)
            {
                sb.AppendLine($"{i.Name},{i.Comment},{i.Price},{i.CollectionName},{i.Condition},{i.Rating},{i.ImagePath},{i.Id},{i.BgColor},{i.CustomFieldName},{i.CustomFieldType},{i.CustomFieldValue}");
            }

            try
            {
                File.WriteAllText(filePath, String.Empty);
                File.AppendAllText(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }

            Debug.WriteLine("\nZapis przedmiotu udany\n");
        }

        public void UpdateItem(Item item)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{item.CollectionName}.txt");
            if (!File.Exists(filePath))
            {
                CreateTextFile(filePath);
            }

            StringBuilder sb = new StringBuilder();

            List<Item> items = LoadItems(item.CollectionName).ToList();

            Item itemToUpdate = items.FirstOrDefault(i => i.Id == item.Id);
            
            if (itemToUpdate != null)
            {
                items.Remove(itemToUpdate);
                itemToUpdate.Name = item.Name;
                itemToUpdate.Comment = item.Comment;
                itemToUpdate.Price = item.Price;
                itemToUpdate.Condition = item.Condition;
                itemToUpdate.Rating = item.Rating;
                itemToUpdate.ImagePath = item.ImagePath;
                if (!String.IsNullOrEmpty(item.ImagePath))
                {
                    string tempPath = item.ImagePath;

                    item.ImagePath = Path.Combine(imagesPath, Path.GetFileName(item.ImagePath));
                    if (!File.Exists(item.ImagePath))
                    {
                        File.Copy(tempPath, Path.Combine(imagesPath, Path.GetFileName(tempPath)));
                    }
                }
                itemToUpdate.BgColor = item.BgColor;
                itemToUpdate.CustomFieldName = item.CustomFieldName;
                itemToUpdate.CustomFieldType = item.CustomFieldType;
                itemToUpdate.CustomFieldValue = item.CustomFieldValue;
                items.Add(itemToUpdate);
            }
            
            sb.AppendLine(string.Join(",", itemHeaders));
            foreach (var _item in items)
            {
                sb.AppendLine($"{_item.Name},{_item.Comment},{_item.Price},{_item.CollectionName},{_item.Condition},{_item.Rating},{_item.ImagePath},{_item.Id},{_item.BgColor},{_item.CustomFieldName},{_item.CustomFieldType},{_item.CustomFieldValue}");
            }

            try
            {
                File.WriteAllText(filePath, String.Empty);
                File.AppendAllText(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }
        }

        public void DeleteItem(Item item)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{item.CollectionName}.txt");
            if (!File.Exists(filePath))
            {
                Debug.WriteLine("Plik nie istnieje");
                return;
            }
            List<Item> items = LoadItems(item.CollectionName).ToList();
            Item itemToDelete = items.FirstOrDefault(i => i.Id == item.Id);
            if (itemToDelete != null)
            {
                items.Remove(itemToDelete);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(",", itemHeaders));

            foreach (var _item in items)
            {
                sb.AppendLine($"{_item.Name},{_item.Comment},{_item.Price},{_item.CollectionName},{_item.Condition},{_item.Rating},{_item.ImagePath},{_item.Id},{_item.BgColor},{_item.CustomFieldName},{_item.CustomFieldType},{_item.CustomFieldValue}");
            }

            try
            {
                File.WriteAllText(filePath, String.Empty);
                File.AppendAllText(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }
        }

        public void DeleteCollection(Collection collection)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{collection.Name}.txt");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            List<Collection> collections = LoadCollections().ToList();
            Collection collectionToDelete = collections.FirstOrDefault(c => c.Name == collection.Name);
            if (collectionToDelete != null)
            {
                collections.Remove(collectionToDelete);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name");
            foreach (var _collection in collections)
            {
                sb.AppendLine($"{_collection.Name}");
            }
            try
            {
                File.WriteAllText(collectionsPath, String.Empty);
                File.AppendAllText(collectionsPath, sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }
        }


        public void ImportItems(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine("Plik nie istnieje");
                return;
            }
            try
            {
                List<Item> items = File.ReadAllLines(filePath)
                    .Skip(1)
                    .Select(csv => Item.FromCsv(csv))
                    .ToList();
                foreach (var item in items)
                {
                    AddItem(item);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("\nBłąd podczas importowania przedmiotów\n");
                throw;
            }

        }
    }
}
