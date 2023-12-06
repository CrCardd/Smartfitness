using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CodeDom;
using System.Drawing;
using System.Linq;
using System.ComponentModel;

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
        Test test = new Test
        {
            Competences = new()
        };

        var lines = data.Split('\n');

        foreach (var line in lines)
            process(line, test);
        
        if (q is not null)
            test.Questions.Add(q);

        return test;
    }

    Question q = null;
    Dictionary<int, Competence> competences = new();
    private void process(string line, Test test)
    {
        if (q is not null)
        {
            if (line.StartsWith("\t"))
                processQuestion(line.Substring(1));
            else
            {
                test.Questions.Add(q);
                q = null;
            }
            return;
        }

        if (line.StartsWith("DataPath"))
        {
            test.DataPath = getInlineValue(line);
            return;
        }

        if (line.StartsWith("c"))
        {
            Competence c = new(getInlineValue(line));
            test.Competences.Add(c);
            competences.Add(getItemValue(line), c);
            return;
        }

        if (line.StartsWith("q"))
        {
            q = new();
            return;
        }
    }

    private void processQuestion(string line)
    {
        
    }

    private int getItemValue(string line)
    {
        var characters = line
            .SkipWhile(c => !char.IsNumber(c))
            .TakeWhile(c => char.IsNumber(c));
        return int.Parse(
            string.Concat(characters)
        );
    }

    private string getInlineValue(string line)
    {
        var characters = line
            .SkipWhile(c => c != '=')
            .Skip(1)
            .SkipWhile(c => char.IsWhiteSpace(c));
        return string.Concat(characters);
    }
}