using System.Linq;
using System.Drawing;

using Pamella;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.Design.Serialization;
using System;

App.Open<MainView>();

public class MainView : View
{
    Test test = null;
    int current = 0;
    int selected = 0;
    bool showImage = false;
    int jump = 80;
    int spacing = 60;
    public MainView()
    {
        
    }

    protected override void OnStart(IGraphics g)
    {
        AlwaysInvalidateMode();
        g.SubscribeKeyDownEvent(async key => {
            if (key == Input.Escape)
                App.Close();
            
            Question question = null;
            int altCount = 0;
            
            switch (key)
            {
                case Input.O:
                    this.test = await Test.LoadFromExamFile("../testExamples/machine learning.exam");
                    break;

                case Input.Down:

                    if (current >= this.test.Questions.Count)
                        break;

                    question = test.Questions[current];
                    altCount = question.Alternatives.Count;

                    selected++;
                    if (selected >= altCount)
                        selected = 0;
                    break;
                
                case Input.Up:

                    if (current >= this.test.Questions.Count)
                        break;

                    question = test.Questions[current];
                    altCount = question.Alternatives.Count;

                    selected--;
                    if (selected < 0)
                        selected = altCount - 1;
                    break;
                
                case Input.I:
                    showImage = !showImage;
                    break;
                
                case Input.W:
                    jump--;
                    break;
                case Input.S:
                    jump++;
                    break;
                
                case Input.Enter:

                    if (current >= this.test.Questions.Count)
                        break;

                    question = test.Questions[current];
                    altCount = question.Alternatives.Count;
                    
                    var alt = question
                        .Alternatives.Keys.ToArray()[selected];
                    var correctness = question.Alternatives[alt];

                    foreach (var comp in question.Competences.Keys)
                        comp.StatusValue += correctness
                            * question.Competences[comp];
                    current++;

                    break;
            }
        });
    }

    protected override void OnRender(IGraphics g)
    {
        g.Clear(Color.White);

        if (this.test is null)
            return;

        if (current >= this.test.Questions.Count)
        {
            showResult(g);
            return;
        }

        var question = this.test.Questions[current];
        var font = new Font("Arial", 40);

        g.DrawText(
            new Rectangle(5, 5, g.Width - 10, g.Height - 10),
            font, StringAlignment.Near, StringAlignment.Near,
            question.Text
        );
        int y = 5 + jump * question.Text.Length / spacing;

        if (question.Images != null && question.Images.Count > 0)
        {
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Near, StringAlignment.Near,
                "Pressione i para ver a imagem"
            );
            y += 40;
        }
        
        if (showImage && question.Images != null && question.Images.Count > 0)
        {
            g.DrawImage(
                new RectangleF(5, 5, g.Width - 10, g.Height - 10),
                question.Images[0]
            );
            return;
        }

        int index = 0;
        foreach (var alt in question.Alternatives.Keys)
        {
            if (selected == index)
                g.FillRectangle(5, y, g.Width - 10, jump * (alt.Length / spacing + 1), Brushes.Black);
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - y - 5),
                font, StringAlignment.Near, StringAlignment.Near,
                selected == index ? Brushes.White : Brushes.Black, alt
            );
            y += jump * (alt.Length / spacing + 1);
            index++;
        }
    }

    private void showResult(IGraphics g)
    {
        int y = 5;
        foreach (var comp in test.Competences)
        {
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Near, StringAlignment.Near,
                comp.Title
            );

            comp.Status = comp.StatusValue switch
            {
                < .4f => CompetenceStatus.Unfit,
                > .4f and < .7f => CompetenceStatus.UnderDevelopment,
                _ => CompetenceStatus.Fit
            };

            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Far, StringAlignment.Near,
                comp.Status switch {
                    CompetenceStatus.Unfit => Brushes.Red,
                    CompetenceStatus.UnderDevelopment => Brushes.Yellow,
                    CompetenceStatus.Fit => Brushes.Green,
                    _ => Brushes.Black
                },
                comp.Status switch {
                    CompetenceStatus.Unfit => "Inapto",
                    CompetenceStatus.UnderDevelopment => "Em Desenvolvimento",
                    CompetenceStatus.Fit => "Apto",
                    _ => ""
                }
            );
            y += 40;
        }
    }
}