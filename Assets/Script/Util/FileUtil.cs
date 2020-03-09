using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// <summary>
// 文件相关通用接口
// </summary>
// <author>zhulin</author>
namespace IGG {
	public class FileUtil  {
		
		// <summary>
		// 确认文件是否存在
		// </summary>
		public static bool CheckFileExist(string fileName) {
			return File.Exists(fileName);
		}

		// <summary>
		// 创建文件夹
		// </summary>
		public  static void CreateFileDirectory(string FilePath)
		{
			if (Directory.Exists(FilePath) == false)
			{
				Directory.CreateDirectory(FilePath);
			}
		}

		// <summary>
		// 清空文件夹
		// </summary>
		public static void ClearFileDirectory(string FileDirectory){

			CreateFileDirectory(FileDirectory);
			List<string> lfile = new List<string>();

			DirectoryInfo rootDirInfo = new DirectoryInfo(FileDirectory);
			foreach (FileInfo file in rootDirInfo.GetFiles())
			{
				lfile.Add(file.FullName);
			}

			foreach (string fileName in lfile) {
				File.Delete(fileName);
			}


			DirectoryInfo rootDic = new DirectoryInfo(FileDirectory);
			foreach (DirectoryInfo fileDic in rootDic.GetDirectories())
			{
				DeleteFileDirectory(fileDic.FullName);
			}

		}

        // 拷贝文件夹
        public delegate bool CopyFilter(string file);
        public static void CopyDirectory(string sourcePath, string destinationPath, string suffix = "", CopyFilter onFilter = null)
        {
            if (onFilter != null && onFilter(sourcePath))
            {
                return;
            }

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            foreach (string file in Directory.GetFileSystemEntries(sourcePath))
            {
                if (File.Exists(file))
                {
                    FileInfo info = new FileInfo(file);
                    if (string.IsNullOrEmpty(suffix) || file.ToLower().EndsWith(suffix.ToLower()))
                    {
                        string destName = Path.Combine(destinationPath, info.Name);
                        if (!(onFilter != null && onFilter(file)))
                        {
                            File.Copy(file, destName);
                        }
                    }
                }

                if (Directory.Exists(file))
                {
                    DirectoryInfo info = new DirectoryInfo(file);
                    string destName = Path.Combine(destinationPath, info.Name);
                    CopyDirectory(file, destName, suffix, onFilter);
                }
            }
        }

        // <summary>
        // 删除文件夹
        // </summary>
        public static void DeleteFileDirectory(string FileDirectory)
        {
            if (Directory.Exists(FileDirectory))
            {
                DirectoryInfo rootDirInfo = new DirectoryInfo(FileDirectory);
                rootDirInfo.Delete(true);
            }
        }

        // 删除文件
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // 拷贝文件
        public static void CopyFile(string pathFrom, string pathTo)
        {
            if (!File.Exists(pathFrom))
            {
                return;
            }

            DeleteFile(pathTo);

            CreateDirectoryFromFile(pathTo);
            File.Copy(pathFrom, pathTo);
        }

        // 根据文件名创建文件所在的目录
        public static void CreateDirectoryFromFile(string path)
        {
            path = path.Replace("\\", "/");
            int index = path.LastIndexOf("/");
            
            string dir = path.Substring(0, index);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        // 获取指定文件下的所有文件
        public static List<string> GetAllChildFiles(string path, string suffix = null, List<string> files = null)
        {
            if (files == null)
            {
                files = new List<string>();
            }

            if (!Directory.Exists(path))
            {
                return files;
            }

            AddFiles(path, suffix, files);

            string[] temps = Directory.GetDirectories(path);
            for (int i = 0; i < temps.Length; ++i)
            {
                string dir = temps[i];
                GetAllChildFiles(dir, suffix, files);
            }

            return files;
        }

        private static void AddFiles(string path, string suffix, List<string> files)
        {
            string[] temps = Directory.GetFiles(path);
            for (int i = 0; i < temps.Length; ++i)
            {
                string file = temps[i];
                if (string.IsNullOrEmpty(suffix) || file.ToLower().EndsWith(suffix.ToLower()))
                {
                    files.Add(file);
                }
            }
        }

        // 保存字节流到文件
        public static void SaveBytesToFile(byte[] bytes, string path)
        {
            CreateDirectoryFromFile(path);

            try
            {
                Stream stream = File.Open(path, FileMode.Create);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // 保存文件到文件
        public static void SaveTextToFile(string text, string path)
        {
            CreateDirectoryFromFile(path);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

            SaveBytesToFile(bytes, path);
        }

        // 以字节流方式读取整个文件
        public static byte[] ReadByteFromFile(string path)
        {
            byte[] bytes = null;

            bool useFileReader = true;
//#if UNITY_ANDROID && !UNITY_EDITOR
//            // 如果是读apk包里的资源,使用Android帮助库加载(目前还没有用到,如果需要,要实现一下java代码,暂时保留)
//            if (path.Contains(Application.streamingAssetsPath))
//            {
//                useFileReader = false;

//                path = path.Replace(Application.streamingAssetsPath + "/", "");
//                bytes = AndroidHelper.FileHelper.CallStatic<byte[]> ("LoadFile", path);
//            }
//#endif
            if (useFileReader && File.Exists(path))
            {
                bytes = File.ReadAllBytes(path);
            }

            return bytes;
        }

        // 以文本方式读取整个文件
        public static string ReadTextFromFile(string path)
        {
            string text = "";

            byte[] bytes = ReadByteFromFile(path);
            if (bytes != null)
            {
                text = System.Text.Encoding.UTF8.GetString(bytes);
            }

            return text;
        }
    }
}


