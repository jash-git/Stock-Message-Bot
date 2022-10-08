using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spider_input
{
    public class FileLib
    {
        //public static string m_StrSysPath = System.Windows.Forms.Application.StartupPath;//WinForm
        //public static string m_StrSysPath = AppDomain.CurrentDomain.BaseDirectory;//Console 與WPF
        public static string m_StrSysPath = Directory.GetCurrentDirectory();//通用
        public static bool IsFileExists(String StrName)// C# 判斷檔案是否存在/檢查檔案是否存在 (C# 確認 檔案 存在) 
        {
            bool blnAns = false;

            String StrFullFileName = String.Format("{0}\\{1}", m_StrSysPath, StrName);

            blnAns = System.IO.File.Exists(StrFullFileName);

            return blnAns;
        }

        public static String ReadTxtFile(String StrFileName, bool blnOneLine = true)
        {
            String StrResult = "";
            String StrFullFileName = String.Format("{0}\\{1}", m_StrSysPath, StrFileName);

            StreamReader sr = new StreamReader(StrFullFileName);
            while (!sr.EndOfStream)
            {
                if (StrResult.Length > 0)
                {
                    StrResult += "\n";
                }

                StrResult += sr.ReadLine();// 寫入文字

                if (blnOneLine)
                {
                    break;
                }
            }
            sr.Close();// 關閉串流
            return StrResult;
        }

        public static void DeleteFile(String StrFileName, bool blnAtRoot = true)
        {
            String FilePath = "";
            if (blnAtRoot == true)
            {
                FilePath = m_StrSysPath + "\\" + StrFileName;
            }
            else
            {
                FilePath = StrFileName;
            }
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    System.IO.File.Delete(FilePath);
                }
                catch
                {
                }
            }
        }
    }
}
