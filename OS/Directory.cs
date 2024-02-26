using System;
using System.Collections.Generic;
using System.Text;

namespace OS_project2
{
    class Directory : Directory_Entry
    {
        public List<Directory_Entry> directoryTable;
        public Directory parent = null;
        public Directory(string namefile, byte attributefile, int firstClustumfile, int size, Directory p): base(namefile,attributefile,firstClustumfile,size)
        {
            directoryTable = new List<Directory_Entry>();
            if (p != null)
            {
                parent = p;
            }
        }

        public Directory_Entry GetDirectory_Entry()
        {
            Directory_Entry me = new Directory_Entry(new string(file_Or_Dir_Name), file_Attribute, fileFirstCluster, fileSize);
            return me;
        }

        public void writeDirectory()
        {
            byte[] directoryTableByte = new byte[directoryTable.Count * 32];
            for (int i = 0; i < directoryTable.Count; i++)
            {
                byte[] derictoryEntryByte = directoryTable[i].GetBytes();
                for (int j = 0, x = i * 32; j < 32 && x < directoryTable.Count * 32; j++, x++)
                {
                    directoryTableByte[x] = derictoryEntryByte[j];
                }
            }
            double numOfBlocks = directoryTableByte.Length / 1024;
            int numOfRequiredBlock = Convert.ToInt32(Math.Ceiling(numOfBlocks));
            int numOfFullSizeBlock = Convert.ToInt32(Math.Floor(numOfBlocks));
            double reminder = directoryTableByte.Length % 1024;
            List<byte[]> allbytes = new List<byte[]>();
            byte[] b = new byte[1024];

            for (int x = 0; x < numOfFullSizeBlock; x++)
            {
                for (int i = 0, j = x * 1024; i < 1024 && j < directoryTableByte.Length; i++, j++)
                {
                    b[i] = directoryTableByte[j];
                }
                allbytes.Add(b);
            }

            int fatIndex;
            if (fileFirstCluster != 0)
            {
                fatIndex = fileFirstCluster;
            }
            else
            {
                fatIndex = Fat_Table.Getavaliableblock();
                fileFirstCluster = fatIndex;
            }
            int lastIndex = -1;
            for (int i = 0; i < allbytes.Count; i++)
            {
                if (fatIndex != -1)
                {
                    Virtual_Disk.WriteCluster(allbytes[i], fatIndex);
                    Fat_Table.set_Next(fatIndex, -1);
                    if (lastIndex != -1)
                    {
                        lastIndex = fatIndex;
                        Fat_Table.set_Next(lastIndex, fatIndex);
                    }
                    fatIndex = Fat_Table.Getavaliableblock();
                    Fat_Table.Write_Fat_Table();
                }
            }
        }



        public void readDirectory()
        {
            if (this.fileFirstCluster != 0)
            {
                int fatIndex = this.fileFirstCluster;

                int next = Fat_Table.get_Next(fatIndex);
                List<byte> ls = new List<byte>();
                List<Directory_Entry> dt = new List<Directory_Entry>();
                do
                {
                    ls.AddRange(Virtual_Disk.Read_Cluster(fatIndex));
                    fatIndex = next;
                    if (fatIndex != -1)
                    {
                        next = Fat_Table.get_Next(fatIndex);
                    }
                } while (next != -1);
                for (int i = 0; i < ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
                    {
                        b[m] = ls[k];
                    }
                    if (b[0] == 0)
                        break;
                    dt.Add(GetDirectoryEntry(b));
                }
            }
        }

        public int searchDirectory(string name)
        {
            if (name.Length < 11)
            {
                name += "\0";
                for (int i = name.Length + 1; i < 12; i++)
                    name += " ";
            }
            else
            {
                name = name.Substring(0, 11);
            }
            for (int i = 0; i < this.directoryTable.Count; i++)
            {
                string n = new string(this.directoryTable[i].file_Or_Dir_Name);
                if (n.Equals(name))
                    return i;
            }
            return -1;
        }

        public int searchFile(string name)
        {
            if (name.Length < 11)
            {
                for (int i = name.Length + 1; i < 12; i++)
                    name += " ";
            }
            else
            {
                name = name.Substring(0, 11);
            }
            for (int i = 0; i < this.directoryTable.Count; i++)
            {
                string n = new string(this.directoryTable[i].file_Or_Dir_Name);
                if (n.Equals(name))
                    return i;
            }
            return -1;
        }

        public void updateContent(Directory_Entry d)
        {
            int index = searchDirectory(new string(d.file_Or_Dir_Name));
            if (index != -1)
            {
                directoryTable.RemoveAt(index);
                directoryTable.Insert(index, d);
            }
        }

        public void deleteDirectory()
        {
            if (this.fileFirstCluster != 0)
            {
                int index = this.fileFirstCluster;
                int next = -1;
                do
                {
                    Fat_Table.set_Next(index, 0);
                    next = index;

                    if (index != -1)
                        index = Fat_Table.get_Next(index);

                } while (next != -1);
            }
            if (this.parent != null)
            {
                parent.readDirectory();
                int Index = parent.searchDirectory(new string(file_Or_Dir_Name));
                if (Index != -1)
                {
                    this.parent.directoryTable.RemoveAt(Index);
                    this.parent.writeDirectory();
                }
            }
        }



    }
}
