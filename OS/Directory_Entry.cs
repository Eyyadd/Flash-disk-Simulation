using System;
using System.Collections.Generic;
using System.Text;

namespace OS_project2
{
    class Directory_Entry
    {
        public char[] file_Or_Dir_Name = new char[11];
        public byte file_Attribute;
        public byte[] fileEmpty = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int fileSize;
        public int fileFirstCluster;

        public Directory_Entry(string name, byte attribute, int firstCluster, int Size)
        {

            this.file_Attribute = attribute;

            if (this.file_Attribute == 0x0)
            {
                string[] filename = name.Split(".");
                assignFileName(filename[0].ToCharArray(), filename[1].ToCharArray());
            }
            else
            {
                assignDIRName(name.ToCharArray());
            }

            this.fileFirstCluster = firstCluster;
            this.fileSize = Size;
        }

        public void assignFileName(char[] name, char[] extension)
        {
            if (name.Length <= 7 && extension.Length == 3)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.file_Or_Dir_Name[i] = name[i];
                }
                
                this.file_Or_Dir_Name[j] = '.';
                for (int i = 0; i < extension.Length; i++)
                {
                    j++;
                    this.file_Or_Dir_Name[j] = extension[i];
                }
                for (int i = ++j; i < file_Or_Dir_Name.Length; i++)
                {
                    this.file_Or_Dir_Name[i] = ' ';
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    this.file_Or_Dir_Name[i] = name[i];
                }
                this.file_Or_Dir_Name[7] = '.';
                for (int i = 0, j = 8; i < extension.Length; j++, i++)
                {
                    this.file_Or_Dir_Name[j] = extension[i];
                }
            }
        }

        public void assignDIRName(char[] name)
        {
            if (name.Length <= 11)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.file_Or_Dir_Name[i] = name[i];
                }
                for (int i = ++j; i < file_Or_Dir_Name.Length; i++)
                {
                    this.file_Or_Dir_Name[i] = ' ';
                }
            }
            else
            {
                int j = 0;
                for (int i = 0; i < 11; i++)
                {
                    j++;
                    this.file_Or_Dir_Name[i] = name[i];
                }
            }
        }

        public byte[] GetBytes()
        {

            byte[] b = new byte[32];

           

            b[11] = file_Attribute;

            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                b[i] = fileEmpty[j];
            }

            for (int i = 24; i < 28; i++)
            {
                b[i] = (byte)fileFirstCluster;
            }

            for (int i = 28; i < 32; i++)
            {
                b[i] = (byte)fileSize;
            }

            return b;
        }

        public Directory_Entry GetDirectoryEntry(byte[] b)
        {

            for (int i = 0; i < 11; i++)
            {
                file_Or_Dir_Name[i] = (char)b[i];
            }

            file_Attribute = b[11];

            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                fileEmpty[j] = b[i];
            }

            for (int i = 24; i < 28; i++)
            {
                fileFirstCluster = b[i];
            }

            for (int i = 28; i < 32; i++)
            {
                fileSize = b[i];
            }

            Directory_Entry dir_1 = new Directory_Entry(new string(file_Or_Dir_Name), file_Attribute, fileFirstCluster, fileSize);
            return dir_1;
        }


    }
}
