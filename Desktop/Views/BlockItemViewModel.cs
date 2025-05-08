using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Desktop.Views
{
    public class BlockItemViewModel : BaseViewModel
    {
        private MainWindowViewModel _mainWindowViewModel;

        public BlockData Data { get; set; }

        private Thickness _blockItemMargin;
        public Thickness BlockItemMargin
        {
            get { return _blockItemMargin; }
            set
            {
                _blockItemMargin = value;
                OnPropertyChanged("BlockItemMargin");
            }
        }

        public BlockItemViewModel(BlockData data, MainWindowViewModel mainWindowViewModel)
        {
            Data = data;
            BlockItemMargin = new Thickness(Data.X, Data.Y, 0, 0);
            _mainWindowViewModel = mainWindowViewModel;
        }


        public void SetMargin(int x, int y)
        {
            Data.X = x;
            Data.Y = y;
            BlockItemMargin = new Thickness(x, y, 0, 0);
        }

        public void Save()
        {
            _mainWindowViewModel.SaveData();
        }

    }
}
