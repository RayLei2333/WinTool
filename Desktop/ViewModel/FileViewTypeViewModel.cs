using Desktop.Commands;
using Desktop.Manager;
using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Desktop.ViewModel
{
    public class FileViewTypeViewModel : BaseViewModel
    {
        private string _blockId;

        #region Porperties

        private ViewType _viewType;
        /// <summary>
        /// 视图类型
        /// </summary>
        public ViewType ViewType
        {
            get { return _viewType; }
            set
            {
                _viewType = value;
                ChangeViewType();
                OnPropertyChanged(nameof(ViewType));
            }
        }

        private ObservableCollection<FileData> _fileData;
        /// <summary>
        /// 文件集合
        /// </summary>
        public ObservableCollection<FileData> FileData
        {
            get { return _fileData; }
            set
            {
                _fileData = value;
                OnPropertyChanged(nameof(FileData));
            }
        }

        #endregion

        #region Command
        public ICommand FileDoubleCickCommand { get; set; }
        #endregion

        public FileViewTypeViewModel(string blockId, List<FileData> fileData)
        {
            FileData = new ObservableCollection<FileData>();
            _blockId = blockId;
            foreach (var item in fileData)
            {
                FileData.Add(item);
            }

            FileDoubleCickCommand = new ReplayCommand<FileData>(FielDoubleClick);
        }



        private void ChangeViewType()
        {
            foreach (var item in FileData)
            {
                string pszFile = item.IsFolder || item.IsLnkFile ? item.FullPath : item.Suffix;
                item.Icon = IconManager.Instence.GetIcon(ViewType, pszFile).Icon;
            }
        }

        private void FielDoubleClick(FileData e)
        {
            try
            {
                // 调用系统默认程序打开文件
                Process.Start(new ProcessStartInfo(e.FullPath)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

    }
}
