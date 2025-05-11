using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Desktop.Views
{
    public class FileViewTypeViewModel : BaseViewModel
    {
        private List<string> _filePaths;
        private List<IconInfo> _icons;

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

        public FileViewTypeViewModel(List<string> filePaths)
        {
            FileData = new ObservableCollection<FileData>();
            _filePaths = filePaths;
            foreach (string path in filePaths)
            {
                bool isFolder = false;
                string suffix = string.Empty;
                if (Directory.Exists(path))
                {
                    isFolder = true;
                }
                else if (File.Exists(path))
                {
                    isFolder = false;
                    suffix = Path.GetExtension(path).ToLower();
                }
                FileData.Add(new FileData()
                {
                    FullPath = path,
                    IsFolder = isFolder,
                    Name = suffix.CheckIsLnkFileSuffix() ? Path.GetFileNameWithoutExtension(path) : Path.GetFileName(path),
                    Suffix = suffix,
                });
            }
        }



        private void ChangeViewType()
        {
            foreach (var item in FileData)
            {
                if (item.IsFolder || item.Suffix.CheckIsLnkFileSuffix())
                {
                    item.Icon = IconManager.Instence.GetIcon(ViewType, item.FullPath).Icon;
                }
                else
                {
                    item.Icon = IconManager.Instence.GetIcon(ViewType, item.Suffix).Icon;
                }

            }
        }

    }
}
