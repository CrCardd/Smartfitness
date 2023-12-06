using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CodeDom;
using System.Drawing;

public class ExamFileReader
{
    private string data;

    public static async Task<ExamFileReader> Open(string path)
    {
        var reader = new ExamFileReader();

        var zip = ZipFile.Open(path, ZipArchiveMode.Read);
        
        foreach (var entry in zip.Entries)
        {
            var isImg = 
                entry.FullName.EndsWith(".png") ||
                entry.FullName.EndsWith(".jpg") ||
                entry.FullName.EndsWith(".bmp");

            var name = entry.Name;
            var lowerName = name.ToLower();
            using var stream = entry.Open();
            
            if (isImg)
            {
                var img = Bitmap.FromStream(stream);
                var imgName = Path.Combine(
                    Environment.CurrentDirectory,
                    entry.Name
                );
                img.Save(imgName);
                continue;
            }
            
            if (lowerName.Contains("exam") && lowerName.EndsWith(".data"))
            {
                using var sr = new StreamReader(stream);
                var code = await sr.ReadToEndAsync();
                sr.Close();
                stream.Close();
                reader.data = code;
                continue;
            }
        }

        return reader;
    }
    private ExamFileReader() { }
    
    public Test GetTest()
    {
        Test test = new Test();


        return null;
    }
}