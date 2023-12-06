using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;

using Csv = System.Collections.Generic.List<System.Collections.Generic.List<string>>;
using System.Runtime.InteropServices;

public enum CompetenceStatus
{
    Unknow = 0,
    Unfit = 1,
    UnderDevelopment = 2,
    Fit = 3,
}

public class Competence
{
    public Competence(string title)
        => this.Title = title;

    public string Title { get; set; }
    public CompetenceStatus Status { get; set; } =  CompetenceStatus.Unknow;
    public float StatusValue { get; set; }
}

public class Question
{
    public string Text { get; set; }
    public List<Image> Images { get; set; }
    public Dictionary<Competence, float> Competences { get; set; }
    public Dictionary<string, float> Alternatives { get; set; }
}

public class Test
{
    private string code;
    public string DataPath { get; set; }
    public string InstanceStudentName { get; set; }
    public List<Question> Questions { get; set; }
    public List<Competence> Competences { get; set; }

    public Test()
    {
        byte[] bytes = new byte[12];
        Random.Shared.NextBytes(bytes);
        this.code = Convert.ToBase64String(bytes);
    }

    public async Task Save()
    {
        var dataFolder = Path.Combine(this.DataPath, "fitness");
        var file = Path.Combine(dataFolder, "test.csv");

        await commit(dataFolder, async () => await save(file));
    }

    private async Task save(string filePath)
    {
        var csv = await getCsv(filePath);
        csv[0].Add(InstanceStudentName);

        for (int i = 1; i < csv.Count; i++)
        {
            var line = csv[i];
            
            var competence = Competences
                .FirstOrDefault(c => c.Title == line[0]);
            line.Add(competence.Status.ToString());
        }

        await setCsv(csv, filePath);
    }

    private async Task setCsv(Csv csv, string filePath)
    {
        string content = "";

        foreach (var line in csv)
        {
            foreach (var column in line)
                content += $"{column};";
            content += "\n";
        }

        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
    }

    private async Task<Csv> getCsv(string filePath)
    {
        Csv csv;
        if (!File.Exists(filePath))
        {
            await File.WriteAllTextAsync(filePath, "");
            csv = new Csv();
            initCsv(csv);
            return csv;
        }

        var lines = await File.ReadAllLinesAsync(filePath);
        csv = lines
            .Select(line => line.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .ToList();
        return csv;
    }

    private void initCsv(Csv csv)
    {
        csv.Add(new () { "Competências" });
        foreach (var c in this.Competences)
        {
            csv.Add(new() {
                c.Title
            });
        }
    }

    private async Task commit(string dataFolder, Func<Task> operation)
    {
        await waitLock(dataFolder);
        await waitPriority(dataFolder);
        await operation();
        await waitUnlock(dataFolder);
    }

    private async Task waitUnlock(string dataFolder)
    {
        while (true)
        {
            var unlocked = tryUnlock(dataFolder);
            if (unlocked)
                break;

            await Task.Delay(50);
        }
    }

    private bool tryUnlock(string dataFolder)
    {
        try
        {
            var file = InstanceStudentName + code + ".lock";
            var location = Path.Combine(dataFolder, file);
            File.Delete(location);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task waitLock(string dataFolder)
    {
        while (true)
        {
            var locked = await tryLock(dataFolder);
            if (locked)
                break;

            await Task.Delay(50);
        }
    }

    private async Task<bool> tryLock(string dataFolder)
    {
        try
        {
            var hasLock = searchLock(dataFolder);
            if (hasLock)
                return false;
            
            var count = lockCount(dataFolder);

            var file = InstanceStudentName + code + ".lock";
            var location = Path.Combine(dataFolder, file);
            await File.WriteAllTextAsync(location, "");
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task waitPriority(string dataFolder)
    {
        while (true)
        {
            var priority = hasPriority(dataFolder);
            if (priority)
                break;
            
            await Task.Delay(50);
        }
    }

    private bool hasPriority(string dataFolder)
    {
        List<string> locks = new List<string>();
        var files = Directory.GetFiles(dataFolder);
        
        foreach (var file in files)
        {
            if (!file.EndsWith(".lock"))
                continue;
            
            locks.Add(file);
        }
        if (locks.Count == 0)
            return true;
        
        locks.Sort();
        var bestPriority = locks[0];
        return bestPriority.Contains(InstanceStudentName + code);
    }

    private bool searchLock(string dataFolder)
        => lockCount(dataFolder) > 0;

    private int lockCount(string dataFolder)
    {
        int count = 0;
        var files = Directory.GetFiles(dataFolder);
        
        foreach (var file in files)
        {
            if (!file.EndsWith(".lock"))
                continue;
            
            count++;
        }

        return count;
    }
}