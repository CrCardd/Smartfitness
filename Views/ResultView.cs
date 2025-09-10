using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Pamella;

public class ResultView : View
{
    private bool HasCalculated = false;

    protected override void OnStart(IGraphics g)
    {
        AlwaysInvalidateMode();

        g.UnsubscribeKeyDownEvent(Context.KeyDownEvent);
        g.UnsubscribeKeyUpEvent(Context.KeyUpEvent);
        Action<Input> KDE = key =>
        {
            switch (key)
            {
                case Input.Escape:
                    {
                        App.Close();
                        break;
                    }
            }
        };
        Context.KeyDownEvent = KDE;
        g.SubscribeKeyDownEvent(KDE);
    }

    protected override async void OnRender(IGraphics g)
    {
        g.Clear(Color.LightSteelBlue);
        int index = 0;

        if (!this.HasCalculated)
        {
            index = 0;
            foreach (var question in Context.Test.Questions)
            {
                if (!Context.AlternativesSelected.Keys.Any(a => a.Equals(index.ToString())))
                    continue;
                var alt_index = Context.AlternativesSelected[index.ToString()];
                var alt = question.AlternativeTexts[alt_index];
                var correctness = question.Alternatives[alt];
                foreach (var comp in question.Competences.Keys)
                    comp.StatusValue += correctness * question.Competences[comp];
                index++;
            }
            this.HasCalculated = true;
        }

        Context.Test.Finish();

        int y = 100;
        index = 0;
        foreach (var comp in Context.Test.Competences)
        {
            bool swap = index % 2 == 0;
            g.FillRectangle(10, y, g.Width - 20, 40, swap ? Brushes.LightGray : Brushes.DimGray);
            g.DrawText(
                new Rectangle(30, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Near, StringAlignment.Near,
                swap ? Brushes.Black : Brushes.White,
                comp.Title
            );

            g.DrawText(
                new Rectangle(5, y, g.Width - 10, 40),
                new Font("Arial", 20), StringAlignment.Far, StringAlignment.Near,
                comp.Status switch
                {
                    CompetenceStatus.Unfit => Brushes.Red,
                    CompetenceStatus.UnderDevelopment => Brushes.Orange,
                    CompetenceStatus.Fit => Brushes.SeaGreen,
                    _ => Brushes.Black
                },
                $"{Math.Round(comp.StatusValue * 100, 1)}%"
            );
            y += 40;
            index++;
        }

        await Context.Test.Save();
    }
}