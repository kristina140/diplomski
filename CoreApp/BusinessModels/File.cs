using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class File
    {
        public File(byte[] content, string name, string fileType)
        {
            Content = content;
            Name = name;
            FileType = fileType;
        }

        public byte[] Content { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
    }
}
