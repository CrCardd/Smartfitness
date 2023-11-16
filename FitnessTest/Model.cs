using System.Drawing;
using System.Collections.Generic;

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
    public List<Question> Questions { get; set; }
    public List<Competence> Competences { get; set; }
}