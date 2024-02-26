using System;
using System.IO;

namespace OS_project2
{
    class cmd
    {
       
        public static void help()
        {
            Console.WriteLine("<< import >>-----------------------------> Add A File From Your Computer To Your Virtual Disk.");
            Console.WriteLine("<< export >>-----------------------------> Add A File From Your Virtual Disk To Your PC.");
            Console.WriteLine("<< rename >>-----------------------------> Update The New Name Of A Directory Or File.");
            Console.WriteLine("<< cls    >>-----------------------------> Clear The Screen.");
            Console.WriteLine("<< quit   >>-----------------------------> Exit The Shell.");
            Console.WriteLine("<< help   >>-----------------------------> Display Information About Each Command.");
            Console.WriteLine("<< type   >>-----------------------------> Display The Content Of A Text File.");
            Console.WriteLine("<< dir    >>-----------------------------> List Of All Files and Sub_Directories in A Directory.");
            Console.WriteLine("<< del    >>-----------------------------> Delete A File.");
            Console.WriteLine("<< md     >>-----------------------------> Create A Directory.");
            Console.WriteLine("<< cd     >>-----------------------------> Change The Current Directory OR Display The Current Working Directory.");
            Console.WriteLine("<< rd     >>-----------------------------> Delete A Directory.");
            



        }
        public static void import(string Path_of_file)
        {
            if (File.Exists(Path_of_file))
            {
                string content = File.ReadAllText(Path_of_file);
                int name_start_with = Path_of_file.LastIndexOf("\\");
                string fil_name = Path_of_file.Substring(name_start_with + 1);
                int indx = Program.Cur_Dir.searchFile(fil_name);
                int F_cluster;
                if (indx == -1)
                {
                    if (content.Length > 0)
                    {
                        F_cluster = Fat_Table.Getavaliableblock();

                    }
                    else
                    {
                        F_cluster = 0;
                    }
                    File_Entry file_1 = new File_Entry(fil_name, 0x0, F_cluster, content.Length, content, Program.Cur_Dir);
                    file_1.writeFileContent();
                    Directory_Entry dir = new Directory_Entry(fil_name, 0x0, F_cluster, content.Length);
                    Program.Cur_Dir.directoryTable.Add(dir);
                    Program.Cur_Dir.writeDirectory();
                }
                else
                {
                    Console.WriteLine("This File is Already Exist.");
                }
            }
            else
            {
                Console.WriteLine("This File is Not Exist.");
            }
        }
        public static void export(string src, string dst)
        {
            int name_start_with = src.LastIndexOf(".");
            string fil_name = src.Substring(name_start_with + 1);
            if (fil_name == "txt")
            {
                int indx = Program.Cur_Dir.searchFile(src);
                if (indx != -1)
                {
                    if (System.IO.Directory.Exists(dst))
                    {
                        int cluster = Program.Cur_Dir.directoryTable[indx].fileFirstCluster;
                        int size = Program.Cur_Dir.directoryTable[indx].fileSize;
                        string content = null;
                        File_Entry file_2 = new File_Entry(src, 0x0, cluster, size, content, Program.Cur_Dir);
                        file_2.readFileContent();
                        StreamWriter X = new StreamWriter(dst + "\\" + src);
                        X.Write(file_2.content);
                        X.Flush();
                        X.Close();
                    }
                    else
                    {
                        Console.WriteLine("The Path is Not Valid.");
                    }
                }
                else
                {
                    Console.WriteLine("The File is Not Exist.");
                }
            }
        }
        public static void rename(string old_Name, string new_Name)
        {
            int oldIndx2 = Program.Cur_Dir.searchDirectory(old_Name);
            int oldIndx = Program.Cur_Dir.searchFile(old_Name);
            if (oldIndx != -1)
            {
                int newIndx = Program.Cur_Dir.searchFile(new string(new_Name));

                if (newIndx == -1)
                {
                    Directory_Entry dir1 = new Directory_Entry(new_Name, Program.Cur_Dir.directoryTable[oldIndx].file_Attribute, Program.Cur_Dir.directoryTable[oldIndx].fileFirstCluster, Program.Cur_Dir.directoryTable[oldIndx].fileSize);

                    Program.Cur_Dir.directoryTable.RemoveAt(oldIndx);
                    Program.Cur_Dir.directoryTable.Insert(oldIndx, dir1);
                    Program.Cur_Dir.writeDirectory();

                }
                else
                {
                    Console.WriteLine("The Name is Alread Exist.");
                }
            }
            else if (oldIndx2 != -1)
            {
                int newIndx2 = Program.Cur_Dir.searchDirectory(new string(new_Name));

                if (newIndx2 == -1)
                {
                    Directory_Entry dir1 = new Directory_Entry(new_Name, Program.Cur_Dir.directoryTable[oldIndx2].file_Attribute, Program.Cur_Dir.directoryTable[oldIndx2].fileFirstCluster, Program.Cur_Dir.directoryTable[oldIndx2].fileSize);
                    Program.Cur_Dir.directoryTable.RemoveAt(oldIndx2);
                    Program.Cur_Dir.directoryTable.Insert(oldIndx2, dir1);
                    Program.Cur_Dir.writeDirectory();

                }
                else
                {
                    Console.WriteLine("The Name is Already Exist.");
                }
            }
            else
            {
                Console.WriteLine("nNo File or Directory Such This Name.");
            }
        }
        public static void cls()
        {
            Console.Clear();
        }

        public static void quit()
        {
            System.Environment.Exit(0);
        }
        public static void type(string Name)
        {
            int indx = Program.Cur_Dir.searchFile(Name);
            if (indx != -1)
            {
                if (Program.Cur_Dir.directoryTable[indx].file_Attribute == 0x0)
                {
                    int F_Cluster = Program.Cur_Dir.directoryTable[indx].fileFirstCluster;
                    int Size = Program.Cur_Dir.directoryTable[indx].fileSize;
                    string Content = string.Empty;
                    File_Entry file = new File_Entry(Name, 0x0, F_Cluster, Size, Content, Program.Cur_Dir);
                    file.readFileContent();
                    Console.WriteLine(file.content);
                }
            }
            else if (Program.Cur_Dir.searchDirectory(Name) != -1)
            { Console.WriteLine("It is A directory Not A File"); }
            else if (indx == -1)
            { Console.WriteLine("File Has Not been Created Yet"); }
        }
        public static void dir()
        {
            int Count_File = 0;
            int Count_Dir = 0;
            int sizeCount = 0;
            Console.WriteLine("Directory of The Partion >>" + new string(Program.Cur_Dir.file_Or_Dir_Name));
            for (int i = 0; i < Program.Cur_Dir.directoryTable.Count; i++)
            {
                if (Program.Cur_Dir.directoryTable[i].file_Attribute == 0x0)
                {
                    Console.WriteLine("------------------>" + Program.Cur_Dir.directoryTable[i].fileSize + "------>" + new string(Program.Cur_Dir.directoryTable[i].file_Or_Dir_Name));
                    Count_File++;
                    sizeCount += Program.Cur_Dir.directoryTable[i].fileSize;
                }
                else
                {
                    Console.WriteLine("------------------>" + "<DIR>" + "------>" + new string(Program.Cur_Dir.directoryTable[i].file_Or_Dir_Name));
                    Count_Dir++;
                }
            }
            Console.WriteLine("------------------>" + Count_File + " File(s)" + "----------->" + sizeCount + " bytes");
            Console.WriteLine("------------------>" + Count_Dir + " Dir(s)" + "------------->" + Fat_Table.GetFreeSpace() + " bytes free");
        }

        public static void md(string Name)
        {
            if (Program.Cur_Dir.searchDirectory(Name) == -1)
            {
                Directory_Entry new_dir = new Directory_Entry(Name, 0x10, 0, 0);
                Program.Cur_Dir.directoryTable.Add(new_dir);
                Program.Cur_Dir.writeDirectory();
                if (Program.Cur_Dir.parent != null)
                {
                    Program.Cur_Dir.parent.updateContent(Program.Cur_Dir.parent);
                    Program.Cur_Dir.parent.writeDirectory();
                }
            }
            else
            {
                Console.WriteLine("The SubDir OR The File "+ Name +" is Alread Exist.");
            }
        }
       
        public static void rd(string Name)
        {
            int indx = Program.Cur_Dir.searchDirectory(Name);
            if (indx != -1)
            {
                int F_Cluster = Program.Cur_Dir.directoryTable[indx].fileFirstCluster;
                Directory dir_1 = new Directory(Name, 0x10, F_Cluster, 0, Program.Cur_Dir);
                dir_1.deleteDirectory();
                Program.Cur_Path = new string(Program.Cur_Dir.file_Or_Dir_Name).Trim();
            }
            else
            {
                Console.WriteLine("The Path is Not Valid.");
            }
        }

        public static void cd(string Name)
        {
            int indx = Program.Cur_Dir.searchDirectory(Name);

            if (indx != -1)
            {
                int F_cluster = Program.Cur_Dir.directoryTable[indx].fileFirstCluster;
                Directory dir_1 = new Directory(Name, 0x10, F_cluster, 0, Program.Cur_Dir);
                Program.Cur_Path = new string(Program.Cur_Dir.file_Or_Dir_Name).Trim() + "\\" + new string(dir_1.file_Or_Dir_Name).Trim();
                Program.Cur_Dir.writeDirectory();
                Program.Cur_Dir.readDirectory();
                Program.Cur_Dir = dir_1;

            }
            else
            {
                Console.WriteLine("The Path is Not Valid.");
            }
        }

        public static void del(string Name_of_File)
        {
            int indx = Program.Cur_Dir.searchFile(Name_of_File);
            if (indx != -1)
            {
                if (Program.Cur_Dir.directoryTable[indx].file_Attribute == 0x0)
                {
                    int cluster = Program.Cur_Dir.directoryTable[indx].fileFirstCluster;
                    int size = Program.Cur_Dir.directoryTable[indx].fileSize;
                    File_Entry file_1 = new File_Entry(Name_of_File, 0x0, cluster, size, null, Program.Cur_Dir);
                    file_1.deleteFile();
                }
                else
                {
                    Console.WriteLine("No File OR Directory Such This Name.");
                }
            }
        }

    }
}
