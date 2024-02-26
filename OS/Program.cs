using System;
using System.IO;
using System.Text;

namespace OS_project2
{
    class Program
    {
        public static Directory Cur_Dir;
        public static string Cur_Path;

        static void Main(string[] args)
        {


            Virtual_Disk.initalize("My_Virt.txt");
            Cur_Path = new string(Cur_Dir.file_Or_Dir_Name);
            while (true)
            {
                Console.Write(Cur_Path.Trim());
                string INPUT = Console.ReadLine();
                if (!INPUT.Contains(" "))
                {
                    if (INPUT.ToLower() == "help")
                    {
                        cmd.help();
                    }
                    else if (INPUT.ToLower() == "quit")
                    {
                        cmd.quit();
                    }
                    else if (INPUT.ToLower() == "cls")
                    {
                        cmd.cls();
                    }
                    else if (INPUT.ToLower() == "md")
                    {
                        Console.WriteLine("Syntax Command Error.");
                    }
                    else if (INPUT.ToLower() == "rd")
                    {
                        Console.WriteLine("Syntax Command Error.");
                    }
                    else if (INPUT.ToLower() == "cd")
                    {
                        if (Cur_Dir.parent != null)
                        {
                            Cur_Dir = Cur_Dir.parent;
                            Cur_Path = new string(Cur_Dir.file_Or_Dir_Name);
                        }
                        Cur_Path = new string(Cur_Dir.file_Or_Dir_Name);
                    }
                    else if (INPUT.ToLower() == "dir")
                    {
                        cmd.dir();
                    }
                    else
                    {
                        Console.WriteLine($"\"{INPUT}\"  There is No Command Such That Command.");
                    }
                }
                else if (INPUT.Contains(" "))
                {
                    string[] lst_of_input = INPUT.Split(" ");
                    if (lst_of_input[0] == "md")
                    {
                        cmd.md(lst_of_input[1]);
                    }
                   
                    else if (lst_of_input[0] == "rd")
                    {
                        cmd.rd(lst_of_input[1]);
                    }
                    else if (lst_of_input[0] == "type")
                    {
                        cmd.type(lst_of_input[1]);
                    }
                    else if (lst_of_input[0] == "del")
                    {
                        cmd.del(lst_of_input[1]);
                    }
                    
                    else if (lst_of_input[0] == "cd")
                    {
                        cmd.cd(lst_of_input[1]);
                    }
                    else if (lst_of_input[0] == "import")
                    {
                        cmd.import(lst_of_input[1]);
                    }
                    else if (lst_of_input[0] == "help")
                    {
                        if (lst_of_input[1] == "md")
                        {
                            Console.WriteLine("<< md >>------------> Create A Directory.");
                        }
                        else if (lst_of_input[1] == "cd")
                        {
                            Console.WriteLine("<< cd >>----------- > Change the Current Directory OR Display The Current Working Directory.");
                        }
                        else if (lst_of_input[1] == "cls")
                        {
                            Console.WriteLine("<< cls >>------------> Clear The Screen.");
                        }
                        else if (lst_of_input[1] == "quit")
                        {
                            Console.WriteLine("<< quit >>-------------> Exit The Shell.");
                        }
                        else if (lst_of_input[1] == "rd")
                        {
                            Console.WriteLine("<< rd >>----------------> Delete A Directory..");
                        }
                        else if (lst_of_input[1] == " ")
                        {
                            cmd.help();
                        }
                    }
                    else if (lst_of_input[0] == "export")
                    {
                        cmd.export(lst_of_input[1], lst_of_input[2]);
                    }
                    else if (lst_of_input[0] == "rename")
                    {
                        cmd.rename(lst_of_input[1], lst_of_input[2]);
                    }

                }
                else
                {
                    Console.WriteLine($"\"{INPUT}\" There is No Command Such That Command.");
                }

            }




        }
    }
}
