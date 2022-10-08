using System.Diagnostics;
using System.Text.Json;

namespace spider_input
{
    static class Program
    {
        public static String m_StrJsonInput = "";
        public static stock_data m_stock_data = null;
        public static bool ReadJsonInput(String FileName = "REQ.json")
        {
            bool blnResult = false;
            String StrLogData;
            m_StrJsonInput = "";
            m_stock_data = null;

            if (File.Exists(FileName))
            {
                StreamReader fileStream = new StreamReader(FileName);
                string line = "";
                while ((line = fileStream.ReadLine()) != null)
                {
                    m_StrJsonInput += line;
                }
                fileStream.Close();
                if ((m_StrJsonInput != null) && (m_StrJsonInput.Length > 0))
                {
                    try
                    {
                        m_stock_data = JsonSerializer.Deserialize<stock_data>(m_StrJsonInput);
                        blnResult = true;
                    }
                    catch
                    {
                        m_stock_data = null;
                        StrLogData = "ReadJsonInput: " + FileName + " parameter data format error";
                        Console.WriteLine(StrLogData);//return;
                        LogFile.Write(StrLogData);
                    }
                }
                else
                {
                    StrLogData = "ReadJsonInput: " + FileName + " parameter data format error";
                    Console.WriteLine(StrLogData);//return;
                    LogFile.Write(StrLogData);
                }
            }
            else
            {
                StrLogData = "ReadJsonInput: " + FileName + " file missing";
                Console.WriteLine(StrLogData);//return;
                LogFile.Write(StrLogData);
            }

            return blnResult;
        }

        public static void Json2SQLite(String Stock_code)
        {
            File.Delete(Stock_code + ".json");
            String StrArguments = String.Format("--no-check-certificate \"https://www.twse.com.tw/exchangeReport/STOCK_DAY?response=json&stockNo={0}\" -O \"{0}.json\"", Stock_code);
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "wget.exe";  // Specify exe name.
            start.Arguments = StrArguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            LogFile.Write("wget Start");
            Process p = Process.Start(start);
            if (p != null)
            {
                p.WaitForExit();
                LogFile.Write("wget Close");
                if (ReadJsonInput($"{Stock_code}.json"))
                {
                    if((m_stock_data!=null) && (m_stock_data.data!=null) && (m_stock_data.data.Count>0))
                    {
                        for (int i = 0; i < m_stock_data.data.Count; i++)
                        {
                            String Time = m_stock_data.data[i][0].Replace("/", "");

                            String Opening_price = m_stock_data.data[i][3];
                            String Highest_price = m_stock_data.data[i][4];
                            String Lowest_price = m_stock_data.data[i][5];
                            String Closing_price = m_stock_data.data[i][6];
                            String Price_difference = m_stock_data.data[i][7];
                            String SQL = String.Format("INSERT INTO main (Time,Stock_code,Opening_price,Highest_price,Lowest_price,Closing_price,Price_difference) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                                                                          Time, Stock_code, Opening_price, Highest_price, Lowest_price, Closing_price, Price_difference);
                            SQLDataTableModel.SQLiteInsertUpdateDelete(SQL);
                        }
                    }
                    else
                    {
                        String StrLogData = Stock_code + ".json" + " data error";
                        Console.WriteLine(StrLogData);//return;
                        LogFile.Write(StrLogData);
                    }

                }
                FileLib.DeleteFile($"{Stock_code}.json");
            }
        }

        public static void pause()
        {
            Console.WriteLine("Press any key to terminate...");
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            LogFile.CleanLog();
            LogFile.Write("spider_input Start");

            if(FileLib.IsFileExists("Stocks.txt"))
            {
                String AllData = FileLib.ReadTxtFile("Stocks.txt", false);
                String[] Stock_codes = AllData.Split('\n');
                for (int i = 0; i < Stock_codes.Length; i++)
                {
                    if (Stock_codes[i].Length > 0)
                    {
                        Console.WriteLine($"{Stock_codes[i]} Start");//return;
                        LogFile.Write($"{Stock_codes[i]} Start");
                        Json2SQLite(Stock_codes[i]);
                        Thread.Sleep(5000);
                        Console.WriteLine($"{Stock_codes[i]} End");//return;
                        LogFile.Write($"{Stock_codes[i]} End");
                    }
                }
            }


            pause();
            LogFile.Write("spider_input Close");
        }
    }
}