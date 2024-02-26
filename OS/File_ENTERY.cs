using System;
using System.Collections.Generic;
using System.Text;

namespace OS_project2
{
    class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
        public File_Entry(string name, byte dir_attr, int dir_firstCluster, int fileSize, string Content, Directory pa) : base(name, dir_attr, dir_firstCluster, fileSize)
        {
            content = Content;
            if (pa != null)
                parent = pa;
        }

        public void writeFileContent()
        {
            byte[] contentBYTES = StringToBytes(content);
            List<byte[]> bytesls = splitBytes(contentBYTES);
            int clusterFATIndex;
            if (this.fileFirstCluster != 0)
            {
                clusterFATIndex = this.fileFirstCluster;
            }
            else
            {
                clusterFATIndex = Fat_Table.Getavaliableblock();
                this.fileFirstCluster = clusterFATIndex;
            }
            int lastCluster = -1;
            for (int i = 0; i < bytesls.Count; i++)
            {
                if (clusterFATIndex != -1)
                {
                    Virtual_Disk.WriteCluster(bytesls[i], clusterFATIndex, 0, bytesls[i].Length);
                    Fat_Table.set_Next(clusterFATIndex, -1);
                    if (lastCluster != -1)
                        Fat_Table.set_Next(lastCluster, clusterFATIndex);
                    lastCluster = clusterFATIndex;
                    clusterFATIndex = Fat_Table.Getavaliableblock();
                }
            }
        }

        List<byte> lsOfBytes = new List<byte>();

        public void readFileContent()
        {
            if (this.fileFirstCluster != 0)
            {
                int cluster = this.fileFirstCluster;
                int next = Fat_Table.get_Next(cluster);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_Disk.Read_Cluster(cluster));
                    cluster = next;
                    if (cluster != -1)
                        next = Fat_Table.get_Next(cluster);
                }
                while (next != -1);
                content = BytesToString(ls.ToArray());
            }

        }

        public void deleteFile()
        {
            if (this.fileFirstCluster != 0)
            {
                int cluster = this.fileFirstCluster;
                int next = Fat_Table.get_Next(cluster);
                do
                {
                    Fat_Table.set_Next(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                        next = Fat_Table.get_Next(cluster);
                }
                while (cluster != -1);
            }
            if (this.parent != null)
            {
                int index = this.parent.searchDirectory(new string(this.file_Or_Dir_Name));
                if (index != -1)
                {
                    this.parent.directoryTable.RemoveAt(index);
                    this.parent.writeDirectory();
                    Fat_Table.Write_Fat_Table();
                }
            }
        }


        public static List<byte[]> splitBytes(byte[] bytes)
        {
            List<byte[]> ls = new List<byte[]>();
            int number_of_arrays = bytes.Length / 1024;
            int rem = bytes.Length % 1024;
            for (int i = 0; i < number_of_arrays; i++)
            {
                byte[] b = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    b[k] = bytes[j];
                }
                ls.Add(b);
            }
            if (rem > 0)
            {
                byte[] b1 = new byte[1024];
                for (int i = number_of_arrays * 1024, k = 0; k < rem; i++, k++)
                {
                    b1[k] = bytes[i];
                }
                ls.Add(b1);
            }
            return ls;
        }

        public static byte[] StringToBytes(string s)
        {
            byte[] bytes = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                bytes[i] = (byte)s[i];
            }
            return bytes;
        }

        public static string BytesToString(byte[] bytes)
        {
            string s = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((char)bytes[i] != '\0')
                    s += (char)bytes[i];
                else
                    break;
            }
            return s;
        }
    }
}


