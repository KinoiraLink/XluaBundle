using System;
using System.IO;

namespace XLuaFramework.Util
{
    public class FileUtil
    {
        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否存在</returns>
        public static bool IsExists(string path)
        {
            FileInfo file = new FileInfo(path);
            return file.Exists;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="data">数据</param>
        public static void WriteFile(string path,byte[] data)
        {
            //获取标准路径
            path = PathUtil.GetStandardPath(path);
            //文件夹的路径
            string dir = path.Substring(0, path.LastIndexOf("/"));

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }
            try
            {
                using FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                fs.Write(data,0,data.Length);
                fs.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}