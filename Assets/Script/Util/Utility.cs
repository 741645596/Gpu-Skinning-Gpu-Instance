using UnityEngine;
using System.Collections;
using System.Diagnostics;

using System.Collections.Generic;

using System;
using System.IO;

namespace IGG {


    public static class FilePaths {
        public const string META_FILE = ".meta";

        // Replacement for Path.Combine(string,string,string,...).  
        // Because the overloads are not implemented in Unity/Mono BCL
        public static string Combine(string pPath, params string[] pSegments) {
            foreach (var segment in pSegments) {
                pPath = Path.Combine(pPath, segment);
            }
            return pPath;
        }

        public static void DeletePaths(string[] paths) {
            if (paths == null || paths.Length == 0) return;

            foreach (string path in paths) {
                DeletePath(path);
            }
        }

        public static void DeletePath(string path) {
            if (string.IsNullOrEmpty(path)) return;

            // Is dir?
            if (Directory.Exists(path) &&
                (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) {
                DeleteDirectory(path);
            } else if (File.Exists(path)) {
                DeleteFile(path);
            }
        }

        private static void DeleteDirectory(string path) {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string file in files) {
                DeleteFile(file);
            }
            foreach (string dir in dirs) {
                DeleteDirectory(dir);
            }
            if (Directory.Exists(path)) {
                Directory.Delete(path);
            }
        }

        public static void DeleteFile(string path) {
            if (File.Exists(path)) {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
        }

        public static void CreateFile(string path, string content, System.Text.Encoding encoding) {
            if (encoding == null) return;

            using (FileStream fs = File.Create(path)) {
                Byte[] info = encoding.GetBytes(content);
                fs.Write(info, 0, info.Length);
            }
        }

        public static string[] GetFilePaths(string path, bool includeMetaFile) {
            string[] paths = null;

            string lastDirOrFileName = GetLastDirectoryOrFileName(path);
            if (lastDirOrFileName.Contains("*")) {
                string dirName = Path.GetDirectoryName(path);

                if (Directory.Exists(dirName)) {
                    // Is file or dir?
                    if (lastDirOrFileName.Contains(".")) {
                        paths = Directory.GetFiles(dirName, lastDirOrFileName);
                    } else {
                        paths = Directory.GetDirectories(dirName, lastDirOrFileName);
                    }
                }
            } else {
                if (Directory.Exists(path) || File.Exists(path)) {
                    paths = new string[1]
                    {
                        path,
                    };
                }
            }

            if (includeMetaFile) {
                return GetFilePathIncludeMetaFile(paths);
            }

            return paths;
        }

        private static string GetLastDirectoryOrFileName(string path) {
            string[] dirOrFileNames = path.Split('/');
            if (dirOrFileNames.Length > 0) {
                return dirOrFileNames[dirOrFileNames.Length - 1];
            }

            return string.Empty;
        }

        private static string[] GetFilePathIncludeMetaFile(string[] paths) {
            if (paths == null) return null;

            List<string> newPaths = new List<string>();
            foreach (string path in paths) {
                newPaths.Add(path);
                string metaFilePath = Path.ChangeExtension(path, META_FILE);
                // Don't need to add meta file path if the original path already exists. (Meta file will be added to newPaths automatically)
                if (Path.GetExtension(path) != META_FILE &&
                    !path.Contains(metaFilePath)) {
                    newPaths.Add(metaFilePath);
                }
            }

            return newPaths.ToArray();
        }

        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName)) {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files) {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
    /* 
	 * Assert and Fail procedures.
	 * 
	 */




        namespace Util {

        public static class Time {
            public static int GetCurrentTimeStampSec() {
                return Mathf.FloorToInt(UnityEngine.Time.realtimeSinceStartup);
            }

            public static float GetTimeAtStartOfCurrentFrame() {
                return UnityEngine.Time.time;
            }

            private static ITime internalTime = new InternalDefaultTime();

            public static ITime Default {
                get {
                    return internalTime;
                }
            }

            // default implementation of ITime. Just return the values from unit.
            private class InternalDefaultTime : ITime {
                public int CurrentFrame {
                    get {
                        return UnityEngine.Time.frameCount;
                    }
                }

                public float DeltaTime {
                    get {
                        return UnityEngine.Time.deltaTime;
                    }
                }

                public float Time {
                    get {
                        return UnityEngine.Time.time;
                    }
                }

                public float FixedDeltaTime {
                    get {
                        return UnityEngine.Time.fixedDeltaTime;
                    }
                }

                public float FixedTime {
                    get {
                        return UnityEngine.Time.fixedTime;
                    }
                }
            }

        } //public static class Time

        public interface ITime {
            int CurrentFrame { get; }
            float DeltaTime { get; }
            float Time { get; }
            float FixedTime { get; }
            float FixedDeltaTime { get; }
        }
    }

}

