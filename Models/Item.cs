using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManager.Models
{
    public class Item
    {
        private int _id = 0;
        private string _name = String.Empty;
        private string _comment = String.Empty;
        private float _price = 0;
        private string _collectionName = String.Empty;
        private string _condition = String.Empty;
        private int _rating = 0;
        private string _imagePath = String.Empty;
        private string _bgColor = String.Empty;
        private string customFieldName = String.Empty;
        private string customFieldValue = String.Empty;
        private string customFieldType = String.Empty;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name 
        {
            get => _name;
            set  
            { 
                _name = value;
                OnPropertyChanged();
            }
        }

        public string CollectionName
        {
            get => _collectionName;
            set  
            { 
                _collectionName = value;
                OnPropertyChanged();
            }
        }

        public string Comment
        {
            get => _comment;
            set  
            { 
                _comment = value;
                OnPropertyChanged();
            }
        }

        public float Price
        {
            get => _price;
            set  
            { 
                _price = value;
                OnPropertyChanged();
            }
        }

        public string Condition
        {
            get => _condition;
            set  
            {
                _condition = value;
                OnPropertyChanged();
            }
        }

        public int Rating
        {
            get => _rating;
            set 
            {
                _rating = value;
                OnPropertyChanged();
            }
        }
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        public string BgColor
        {
            get => _bgColor;
            set
            {
                _bgColor = value;
                OnPropertyChanged();
            }
        }
        public string CustomFieldName
        {
            get => customFieldName;
            set
            {
                customFieldName = value;
                OnPropertyChanged();
            }
        }

        public string CustomFieldValue
        {
            get => customFieldValue;
            set
            {
                customFieldValue = value;
                OnPropertyChanged();
            }
        }

        public string CustomFieldType
        {
            get => customFieldType;
            set
            {
                customFieldType = value;
                OnPropertyChanged();
            }
        }

        static public Item FromCsv(string csv)
        {
            string[] lines = csv.Split(',');
            Item _item = new();

            _item.Name = lines[0];
            _item.CollectionName = lines[3];
            _item.Comment = lines[1];
            _item.Price = float.Parse(lines[2]);
            _item.Condition = lines[4];
            _item.Rating = int.Parse(lines[5]);
            _item.ImagePath = lines[6];
            _item.Id = int.Parse(lines[7]);
            _item.BgColor = lines[8];
            _item.CustomFieldName = lines[9];
            _item.CustomFieldType = lines[10];
            _item.CustomFieldValue = lines[11];
            return _item;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
