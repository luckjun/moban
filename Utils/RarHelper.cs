using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Moban.Utils
{
    public class RarHelper
    {

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="unRarPatch">rar所在目录</param>
        /// <param name="rarPatch">要解压到的目录</param>
        /// <param name="rarName">rar文件名</param>
        /// <param name="deleteAfterUnCompress">解压结束后是否删除原文件</param>
        /// <returns></returns>
        public static bool UnCompressRAR(string unRarPatch, string rarPatch, string rarName, bool deleteAfterUnCompress)
        {
            string the_Info;
            try
            {

                #region 开始解压文件前的检查和准备

                //验证传输人的参数不为null和空值
                if (unRarPatch == null || rarPatch == null || rarName == null || unRarPatch == string.Empty || rarPatch == string.Empty || rarName == string.Empty)
                {
                    return false;
                }

                if (File.Exists(unRarPatch + rarName) == false)
                {
                    return false;//要解压的文件夹不存在
                }

                if (Directory.Exists(unRarPatch) == false)
                {
                    Directory.CreateDirectory(unRarPatch);
                }

                #endregion

                #region 开始解压文件

                the_Info = "x \"" + rarName + "\" \"" + unRarPatch + "\" -y";

                ProcessStartInfo the_StartInfo = new ProcessStartInfo();
                the_StartInfo.RedirectStandardOutput = true;
                the_StartInfo.UseShellExecute = false;
                //the_StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"bin\rar.exe";
                the_StartInfo.FileName = @"E:\C#\GlobalError\UnitTest\RarHelperTest\bin\Debug\rar.exe";
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_StartInfo.WorkingDirectory = rarPatch;//获取压缩包路径


                Process the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();

                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            //解压后删除
            if (deleteAfterUnCompress)
            {
                if (File.Exists(rarPatch + "\\" + rarName))
                    File.Delete(rarPatch + "\\" + rarName);
            }

            return true;
        }


        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="compressPath">要压缩的文件夹名</param>
        /// <param name="rarName">压缩后的文件名</param>
        /// <param name="rarPath">压缩后所在的路径名</param>
        /// <returns></returns>
        public static bool CompressRAR(string compressPath, string rarName, string rarPath)
        {
            string the_Info;
            try
            {

                #region 开始压缩文件前的检查和准备

                //验证传输人的参数不为null和空值
                if (compressPath == null || rarName == null || rarPath == null || compressPath == string.Empty || rarName == string.Empty || rarPath == string.Empty)
                {
                    return false;
                }
                if (Directory.Exists(compressPath) == false)
                {
                    return false;//要压缩的文件夹不存在
                }
                if (Directory.Exists(rarPath) == false)
                {
                    Directory.CreateDirectory(rarPath);
                }

                #endregion

                #region 开始压缩文件

                the_Info = @" a -k -r -s -m1 {0} {1} ";
                the_Info = String.Format(the_Info, rarPath + rarName, compressPath);

                ProcessStartInfo the_StartInfo = new ProcessStartInfo();
                the_StartInfo.RedirectStandardOutput = true;
                the_StartInfo.UseShellExecute = false;
                //the_StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"bin\rar.exe";
                the_StartInfo.FileName = @"E:\C#\GlobalError\UnitTest\RarHelperTest\bin\Debug\rar.exe";
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //the_StartInfo.WorkingDirectory = rarPatch;//获取压缩包路径

                Process the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();

                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

    }
}
