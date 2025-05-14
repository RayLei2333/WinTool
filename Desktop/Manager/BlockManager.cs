using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop.Manager
{
    /// <summary>
    /// 桌面分块管理器
    /// </summary>
    class BlockManager
    {
        private static BlockManager _instence = new BlockManager();

        public static BlockManager Instence => _instence;

        /// <summary>
        /// 桌面分块数据
        /// </summary>
        public List<BlockData> BlockData { get; set; }

        public BlockManager()
        {
            Init();
        }

        /// <summary>
        /// 添加分块
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public BlockData AddBlock(int x, int y, int width, int height, string name)
        {
            BlockData block = new BlockData()
            {
                Id = Guid.NewGuid().ToString(),
                X = x,
                Y = y,
                Width = width,
                Height = height,
                ViewType = ViewType.LargeIcon,
                Lock = false,
                Name = name
            };
            BlockData.Add(block);
            return block;
        }

        public BlockData AddBlock(int x, string name)
        {
            int width = (int)(SystemParameters.WorkArea.Width * 0.2);
            int heidht = (int)(SystemParameters.WorkArea.Height * 0.9);
            int y = (int)((SystemParameters.WorkArea.Height - heidht) / 2);
            return AddBlock(x, y, width, heidht, name);
        }


        /// <summary>
        /// 移除分块
        /// </summary>
        /// <param name="id">分块id</param>
        /// <returns></returns>
        public BlockData RemoveBlock(string id)
        {
            var block = BlockData.FirstOrDefault(b => b.Id == id);
            BlockData.Remove(block);
            return block;
        }

        public void ResetName(string id, string name)
        {
            BlockData.FirstOrDefault(b => b.Id == id).Name = name;
        }

        public void Init()
        {
            if (!File.Exists("data.json"))
                BlockData = new List<BlockData>();
            else
            {
                string json = File.ReadAllText("data.json");
                BlockData = JsonSerializer.Deserialize<List<BlockData>>(json);
                //foreach (var block in BlockData)
                //{
                //    block.InitFileList();
                //}
            }
        }

        public void ResetFileList()
        {
            foreach (var item in BlockData)
            {
                ResetFileList(item.Id);
            }
        }

        public void ResetFileList(string blockId)
        {
            var block = BlockData.FirstOrDefault(b => b.Id == blockId);
            if (block == null)
                return;

            if (block.FilePathList.Any())
            {
                if (block.FileList.Any())
                    block.FileList.Clear();
                foreach (var filePath in block.FilePathList)
                {
                    if (!Directory.Exists(filePath) && !File.Exists(filePath))
                        continue;
                    FileData fileData = new FileData()
                    {
                        FullPath = filePath,
                        IsFolder = Directory.Exists(filePath),
                    };
                    if (fileData.IsFolder)
                    {
                        fileData.Name = Path.GetFileName(filePath);
                    }
                    else
                    {
                        fileData.Suffix = Path.GetExtension(filePath).ToLower();
                        fileData.IsLnkFile = fileData.Suffix.CheckIsLnkFileSuffix();
                        fileData.IsImageFile = fileData.Suffix.CheckIsImgFileSuffix();
                        if (fileData.IsLnkFile)
                            fileData.Name = Path.GetFileNameWithoutExtension(filePath);
                        else
                            fileData.Name = Path.GetFileName(filePath);
                    }

                    block.FileList.Add(fileData);
                }
            }
        }


        public void Save()
        {
            string json = JsonSerializer.Serialize(BlockData);
            File.WriteAllText("data.json", json);
        }

    }
}
