using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;


namespace url_generator
{
    class Program
    {
        static void Main(string[] args)
        {
            bool debug = true;
            int line_number;
            string crash_line, true_url_without_line_number, true_url_with_line_number,lua_path = "";
            string base_url = "https://github.com/steam-test1/Payday-2-LuaJit-Source-With-Line-Numbers-Continued/blob/master/";
            crash_line = Console.ReadLine();
            if (debug) Console.WriteLine("debug:crash_line=" + crash_line);
            if (crash_line.Length == 0) return;
            //rule out mod path
            if (crash_line.Contains("@mods/") || crash_line.Contains("mods/"))
            {
                Console.WriteLine("no valid url for mods.");
                Console.ReadLine();
                return;
            }
            //make the url
            if (crash_line.IndexOf("core/") != -1)lua_path = func(crash_line, crash_line.IndexOf("core/"));
            else
            {
                if (crash_line.IndexOf("lib/") != -1) lua_path = func(crash_line, crash_line.IndexOf("lib/"));
                else
                {
                    Console.WriteLine("no vaild path at this line!");
                    Console.ReadLine();
                    return;
                }
            }
            if (debug) Console.WriteLine("debug:lua_path=" + lua_path);
            true_url_without_line_number = String.Concat(base_url, lua_path);
            if (debug) Console.WriteLine("debug:true_url_without_line_number=" + true_url_without_line_number);
            Console.WriteLine(true_url_without_line_number);
            //get the line number
            line_number= int.Parse(Regex.Replace(crash_line, @"[^0-9]+", ""));
            if(debug) Console.WriteLine("debug:line_number=" + line_number);
            //get http info
            HttpWebRequest hwrqe = (HttpWebRequest)WebRequest.Create(true_url_without_line_number);
            hwrqe.KeepAlive = false;
            hwrqe.Timeout = 20000;//ms
            hwrqe.Method = "get";
            hwrqe.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            HttpWebResponse hwrs = (HttpWebResponse)hwrqe.GetResponse();
            if (hwrs.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            if (debug) Console.WriteLine("a");
            using (StreamReader sr = new StreamReader(hwrs.GetResponseStream()))
            {
                while(sr.Peek()>=0)
                {
                    string line = sr.ReadLine();
                    if (debug) Console.WriteLine(line);
                    int a = line.IndexOf("--</span> Lines ");
                    if (a!=-1)
                    {
                        if (debug) Console.WriteLine("debug:line=" + line);
                        string temp = line.Substring(a + 16);
                        temp = temp.Remove(temp.IndexOf("<"));
                        int min, max;
                        if (debug) Console.WriteLine(temp.Substring(0, temp.IndexOf("-") - 1));
                        min = int.Parse(temp.Substring(0, temp.IndexOf("-")-1));
                        max = int.Parse(temp.Substring(temp.IndexOf("-")+1));
                        if (debug) Console.WriteLine("debug:max=" + max+",min="+min);
                        if (line_number >= min && line_number<= max)
                        {
                            temp = line.Remove(line.IndexOf("class"));
                            true_url_with_line_number = String.Concat(true_url_without_line_number, "#L", Regex.Replace(temp, @"[^0-9]+", ""));
                            Console.WriteLine(true_url_with_line_number);
                        }
                    }
                }
                
            }
            



            Console.ReadLine();
        }

        static string func(string crash_line, int index)
        {
            string a;
            crash_line = crash_line.Substring(index);
            a= crash_line.Remove(crash_line.IndexOf(".lua") + 4);
            return a;
        }
    }
}
