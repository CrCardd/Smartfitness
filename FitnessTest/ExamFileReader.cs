using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            Competences = new(),
            Questions = new()
        };

        IEnumerable<string> lines = data.Split('\n');
        var it = lines.GetEnumerator();

        process(lines.GetEnumerator(), test);

        test.Questions = test.Questions
            .OrderBy(q => Random.Shared.Next())
            .ToList();
        
        foreach (var question in test.Questions)
        {
            question.AlternativeTexts = 
                question.Alternatives.Keys
                .OrderBy(q => Random.Shared.Next())
                .ToList();
        }

        return test;
    }

    Dictionary<int, Competence> competences = new();
    private void process(IEnumerator<string> it, Test test)
    {
        while (it.MoveNext())
        {
            var line = it.Current;

            if (line.StartsWith("DataPath"))
            {
                test.DataPath = getInlineValue(line);
                continue;
            }

            if (line.StartsWith("c"))
            {
                Competence c = new(getInlineValue(line));
                test.Competences.Add(c);
                competences.Add(getItemValue(line), c);
                continue;
            }

            if (line.StartsWith("q"))
            {
                var question = processQuestion(it);
                test.Questions.Add(question);
                continue;
            }
        }
    }

    private Question processQuestion(IEnumerator<string> it)
    {
        Question question = new()
        {
            Alternatives = new(),
            Competences = new()
        };
        
        bool bypass = false;
        while (bypass || it.MoveNext())
        {
            if (bypass)
                bypass = false;
            
            var line = it.Current;
            if (line.Length == 0)
                return question;

            int currentLevel = 
                line[0] == '\t'
                    ? 4 * line
                        .TakeWhile(c => c == '\t')
                        .Count()
                    : line
                        .TakeWhile(c => c == ' ')
                        .Count();
            
            if (currentLevel < 4)
                return question;
            
            line = line.Trim();

            if (line.StartsWith("Text"))
            {
                var value = getInlineValue(line);
                if (value == string.Empty)
                {
                    value = getValue(it, 8);
                    bypass = true;
                }
                question.Text = value;
                continue;
            }

            if (line.StartsWith("Images"))
            {
                question.Images = new List<Image>
                {
                    Bitmap.FromFile(getInlineValue(line).Trim())
                };
                continue;                
            }

            if (line.StartsWith("Competences"))
            {
                var newLevel = fillCompetences(question.Competences, it, 8);
                if (newLevel == 0)
                    return question;
                continue;
            }


            if (line.StartsWith("Alternatives"))
            {
                var newLevel = fillAlternatives(question.Alternatives, it, 8);
                if (newLevel == 0)
                    return question;
                continue;
            }
        }
        
        return question;
    }

    private int fillAlternatives(Dictionary<string, float> dict, IEnumerator<string> it, int level)
    {
        while (it.MoveNext())
        {
            var line = it.Current;
            if (line.Length == 0)
                return 0;

            int currentLevel = 
                line[0] == '\t'
                    ? 4 * line
                        .TakeWhile(c => c == '\t')
                        .Count()
                    : line
                        .TakeWhile(c => c == ' ')
                        .Count();

            if (currentLevel != level)
                return currentLevel;
            
            var alt = string.Concat(line
                .TakeWhile(c => c != '=')
            ).Trim();

            var weight = int.Parse(
                string.Concat(
                    line
                        .SkipWhile(c => c != '=')
                        .Skip(1)
                ).Trim()
            );

            dict[alt] = weight;
        }
        return 0;
    }

    private int fillCompetences(Dictionary<Competence, float> dict, IEnumerator<string> it, int level)
    {
        int count = 0;
        List<Competence> comps = new List<Competence>();
        while (it.MoveNext())
        {
            var line = it.Current;
            if (line.Length == 0)
                return 0;

            int currentLevel = 
                line[0] == '\t'
                    ? 4 * line
                        .TakeWhile(c => c == '\t')
                        .Count()
                    : line
                        .TakeWhile(c => c == ' ')
                        .Count();

            if (currentLevel != level)
                return currentLevel;
            
            var text = line
                .Trim();
            int compId = int.Parse(string.Concat(text
                .SkipWhile(c => !char.IsNumber(c))
                .TakeWhile(c => char.IsNumber(c))
            ));
            var comp = this.competences[compId];
            comps.Add(comp);

            var weight = int.Parse(getInlineValue(line));
            count += weight;

            dict[comp] = weight;
        }

        foreach (var comp in comps)
        {
            dict[comp] /= (float)count;
        }
        return 0;
    }

    private string getValue(IEnumerator<string> it, int level)
    {
        StringBuilder sb = new StringBuilder();
        
        while (it.MoveNext())
        {
            var line = it.Current;

            int currentLevel = 
                line[0] == '\t' ? 
                    4 * line
                        .TakeWhile(c => c == '\t')
                        .Count() :
                    line
                    .TakeWhile(c => c == ' ')
                    .Count();

            if (currentLevel != level)
                break;
            
            var text = line
                .Replace("newline", "\n")
                .Trim();
            sb.Append(text);
            sb.Append(" ");
        }

        return sb.ToString();
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
        return string.Concat(characters).Trim();
    }
}