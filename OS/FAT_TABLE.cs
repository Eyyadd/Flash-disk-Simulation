using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OS_project2
{
    class Fat_Table
    {
        static int[] fat_table = new int[1024];
        static byte[] arrOfByte = new byte[4 * 1024];


        public Fat_Table()
        {
            fat_table = new int[1024];
        }

        public static void initialize()
        {
            for (int i = 0; i < fat_table.Length; i++)
            {
                if (i < 5)
                    fat_table[i] = -1;
                else
                    fat_table[i] = 0;
            }

        }

        public static void Write_Fat_Table()
        {
            Virtual_Disk.Disk = new FileStream("MyData.txt", FileMode.Open, FileAccess.Write);
            Virtual_Disk.Disk.Seek(1024, SeekOrigin.Begin);
            Buffer.BlockCopy(fat_table, 0, arrOfByte, 0, arrOfByte.Length);

            Virtual_Disk.Disk.Write(arrOfByte, 0, arrOfByte.Length);
            Virtual_Disk.Disk.Close();


        }


        public static int[] get_fat_table()
        {
            Virtual_Disk.Disk = new FileStream("MyData.txt", FileMode.Open, FileAccess.Read);
            Virtual_Disk.Disk.Seek(1024, SeekOrigin.Begin);
            Virtual_Disk.Disk.Read(arrOfByte, 0, arrOfByte.Length);
            Buffer.BlockCopy(arrOfByte, 0, fat_table, 0, 4096);
            Virtual_Disk.Disk.Close();
            return (fat_table);
        }


        public void Print_fat_table()
        {

            get_fat_table();

            for (int i = 0; i < fat_table.Length; i++)
            {
                Console.WriteLine((i + 1) + "------------------------>\t" + fat_table[i]);
            }

        }


        public static int Getavaliableblock()
        {
            int freeIndex = -1;
            for (int i = 0; i < 1024; i++)
            {
                if (fat_table[i] == 0)
                {
                    freeIndex = i;
                    break;
                }

            }
            return freeIndex;
        }

        public static int GetAvilaibleBlocks()
        {
            int count = 0;
            for (int i = 0; i < fat_table.Length; i++)
            {
                if (fat_table[i] == 0)
                {
                    count++;
                }
            }
            return count;
        }

        public static int GetFreeSpace()
        {
            return GetAvilaibleBlocks() * 1024;
        }

        public static int get_Next(int index)
        {
            return (fat_table[index]);
        }

        // put the value in this index
        public static void set_Next(int index, int value)
        {
            fat_table[index] = value;
        }

    }

}
