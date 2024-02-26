using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OS_project2
{
    class Virtual_Disk
    {
        public static FileStream Disk;

        public static void CREATE_Disk(string path)
        {
            Disk = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            Disk.Close();
        }

        public static int getFreeSpace()
        {
            return (1024 * 1024) - (int)Disk.Length;
        }

        public static void initalize(string path)
        {
            if (!File.Exists(path))
            {
                CREATE_Disk(path);
                byte[] b = new byte[1024];
                for (int i = 0; i < b.Length; i++)
                {
                    b[i] = 0;
                }

                WriteCluster(b, 0);
                Fat_Table f = new Fat_Table();
                Fat_Table.initialize();
                Directory root = new Directory("E:>> ", 0x10, 5, 0, null);

                root.writeDirectory();
                Fat_Table.set_Next(5, -1);
                Program.Cur_Dir = root;
                Fat_Table.Write_Fat_Table();
            }
            else
            {

                Fat_Table.get_fat_table();
                Directory root = new Directory("E:>> ", 0x10, 5, 0, null);
                root.readDirectory();
                Program.Cur_Dir = root;
            }
        }

        public static void WriteCluster(byte[] data, int Index, int offset = 0, int count = 1024)
        {
            Disk = new FileStream("MyData.txt", FileMode.Open, FileAccess.Write);
            Disk.Seek(Index * 1024, SeekOrigin.Begin);
            Disk.Write(data, offset, count);
            Disk.Flush();
            Disk.Close();
        }

        public static byte[] Read_Cluster(int clusterIndex)
        {
            Disk = new FileStream("MyData.txt", FileMode.Open, FileAccess.Read);
            Disk.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            Disk.Read(bytes, 0, 1024);
            Disk.Close();
            return bytes;
        }
    }
}
