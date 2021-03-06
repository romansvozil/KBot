﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KBot.CLI.Encryption;
using KBot.CLI.Files;

namespace KBot.CLI.Openers
{
    public static class TextOpener
    {
        public static IEnumerable<TextFile> Open(string path)
        {
            var files = new List<TextFile>();
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    int index = reader.ReadInt32();
                    int nameSize = reader.ReadInt32();
                    string name = Encoding.Default.GetString(reader.ReadBytes(nameSize));
                    bool dat = Convert.ToBoolean(reader.ReadInt32());
                    int fileSize = reader.ReadInt32();
                    byte[] content = reader.ReadBytes(fileSize);
                    
                    byte[] decrypted;
                    if (dat || name.EndsWith(".dat"))
                    {
                        decrypted = Dat.Decrypt(content);
                    }
                    else
                    {
                        decrypted = Lst.Decrypt(content);
                    }
                    
                    files.Add(new TextFile
                    {
                        Name = name,
                        Content = decrypted
                    });
                }
            }
            
            return files;
        }
    }
}