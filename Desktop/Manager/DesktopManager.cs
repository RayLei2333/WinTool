using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Desktop.Win32Support;

namespace Desktop.Manager
{
    internal class DesktopManager
    {
        private static DesktopManager _instence = new DesktopManager();

        public static DesktopManager Instence => _instence;

        /// <summary>
        /// 是否开启监控
        /// </summary>
        public bool StartedWatcher { get; set; }

        public DesktopData DesktopData { get; private set; }

        private FileSystemWatcher[] _watchers = new FileSystemWatcher[2];

        //监控桌面文件夹路径
        private readonly string[] watchPaths =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        };

        public event FileSystemEventHandler FileDeleted;
        public event FileSystemEventHandler FileCreated;
        public event FileSystemEventHandler FileChanged;
        public event RenamedEventHandler FileRenamed;
        public event ErrorEventHandler FileError;



        public DesktopManager()
        {

            //判断有没有数据文件
            if (File.Exists(Constants.DesktopDataPath))
            {
                //有则读取
                DesktopData = JsonSerializer.Deserialize<DesktopData>(File.ReadAllText(Constants.DesktopDataPath));
            }
            else
            {
                //创建数据
                GetDesktopData();
            }

            InitWatcher();
        }

        private void GetDesktopData()
        {
            var iconView = DesktopWindow.GetDesktopIconView();
            DesktopData = new DesktopData()
            {
                AutoArrange = iconView.AutoArrange,
                AlignedToGrid = iconView.AlignedToGrid,
                IconWidth = iconView.X,
                IconHeight = iconView.Y
            };

            //获取桌面图标数据
            var file = DesktopWindow.GetDesktopFiles();

            foreach (var item in file)
            {
                string extension = System.IO.Path.GetExtension(item.FilePath);
                DesktopData.FileData.Add(new DesktopFileData()
                {
                    FullPath = item.FilePath,
                    X = item.X,
                    Y = item.Y,
                    Seq = item.Index,
                    Name = item.Name,
                    IsFolder = Directory.Exists(item.FilePath),
                    IsLnkFile = extension.CheckIsLnkFileSuffix(),
                    IsImageFile = extension.CheckIsImgFileSuffix(),
                    Suffix = extension,
                });
            }
        }

        #region File Watcher

        public void InitWatcher()
        {
            for (int i = 0; i < watchPaths.Length; i++)
            {
                var watcher = new FileSystemWatcher();
                watcher.Path = watchPaths[i];
                watcher.Filter = "*.*";
                watcher.Error += OnFileError;
                watcher.Changed+= OnFileChanged;
                watcher.Created += OnFileCreate;
                watcher.Deleted += OnFileDelete;
                watcher.Renamed += OnFileRenamed;
                _watchers[i] = watcher;
            }
        }

        /// <summary>
        /// 开启监控
        /// </summary>
        public void StartWatcher()
        {
            StartedWatcher = true;
            foreach (var item in _watchers)
                item.EnableRaisingEvents = StartedWatcher;
        }

        /// <summary>
        /// 关闭监控
        /// </summary>
        public void StopWatcher()
        {
            StartedWatcher = false;
            foreach (var item in _watchers)
                item.EnableRaisingEvents = StartedWatcher;

        }

        private void OnFileCreate(object sender, FileSystemEventArgs e)
        {
            if (FileCreated != null)
                FileChanged.Invoke(this, e);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (FileChanged != null)
                FileChanged.Invoke(sender, e);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (FileRenamed != null)
                FileRenamed.Invoke(sender, e);
        }

        private void OnFileDelete(object sender, FileSystemEventArgs e)
        {
            if (FileDeleted != null)
                FileDeleted.Invoke(sender, e);
        }

        private void OnFileError(object sender, ErrorEventArgs e)
        {
            if (FileError != null)
                FileError.Invoke(sender, e);
        }

        #endregion

        public void SaveDesktopData()
        {
            if (!File.Exists(Constants.DesktopDataPath))
                File.Create(Constants.DesktopDataPath).Dispose();

            File.WriteAllText(Constants.DesktopDataPath, JsonSerializer.Serialize(DesktopData));
        }
    }
}
