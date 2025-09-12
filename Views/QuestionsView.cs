using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Pamella;

public class QuestionsView : View
{

    private int Jump { get; set; } = 80;
    private int Spacing { get; set; } = 60;

    private int AlternativeSelected { get; set; }
    private bool ShowImage { get; set; }
    private int Current { get; set; }

    protected override void OnStart(IGraphics g)
    {
        AlwaysInvalidateMode();
        
        g.UnsubscribeKeyDownEvent(Context.KeyDownEvent);
        g.UnsubscribeKeyUpEvent(Context.KeyUpEvent);
        Action<Input> KDE = key =>
        {
            var question = Context.Test.Questions[Context.Current];

            switch (key)
            {
                case Input.Up:
                    {
                        if (this.ShowImage)
                            break;
                        if (--this.AlternativeSelected < 0)
                            this.AlternativeSelected = question.Alternatives.Count - 1;
                        break;
                    }
                case Input.Down:
                    {
                        if (this.ShowImage)
                            break;
                        if (++this.AlternativeSelected > question.Alternatives.Count - 1)
                            this.AlternativeSelected = 0;
                        break;
                    }
                case Input.Left:
                    {
                        if (Context.Current == 0)
                            break;
                        if (this.ShowImage)
                            break;
                        Context.Current--;
                        break;
                    }
                case Input.Right:
                    {
                        if (Context.Current == Context.Test.Questions.Count-1)
                            break;
                        if (this.ShowImage)
                            break;
                        Context.Current++;
                        break;
                    }
                case Input.Enter:
                    {
                        var alt = question.AlternativeTexts[this.AlternativeSelected];
                        Context.AlternativesSelected[Context.Current.ToString()] = this.AlternativeSelected;
                        if (Context.Current != Context.Test.Questions.Count - 1)
                            Context.Current++;
                        break;
                    }
                case Input.I:
                    {
                        this.ShowImage = !this.ShowImage;
                        break;
                    }
                case Input.W:
                    {
                        this.Jump--;
                        break;
                    }
                case Input.S:
                    {
                        this.Jump++;
                        break;
                    }
                case Input.F:
                    {
                        App.Push(new JumpView(new ResultView(), Input.F, "FINALIZAR"));
                        return;
                    }
            }
        };
        Context.KeyDownEvent = KDE;
        g.SubscribeKeyDownEvent(KDE);
    }

    protected override void OnRender(IGraphics g)
    {
        g.Clear(Context.BackgroundColor);
        var question = Context.Test.Questions[Context.Current];
        var font = new Font("Arial", 30);

        g.DrawText(
            new Rectangle(5, 100, g.Width - 10, g.Height - 10),
            font, StringAlignment.Center, StringAlignment.Near,
            question.Text
        );

        g.DrawText(
            new Rectangle(10, 10, g.Width - 10, g.Height - 10),
            font, StringAlignment.Near, StringAlignment.Near,
            $"{Context.Current + 1}/{Context.Test.Questions.Count}"
        );

        TimeSpan endts = Context.StartTime.AddHours(2).ToTimeSpan();
        TimeSpan nowts = TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan();
        TimeSpan timeleft = endts - nowts;
        if (Context.StartTime.AddHours(2) == TimeOnly.FromDateTime(DateTime.Now))
            App.Push(new ResultView());


            g.DrawText(
                new Rectangle(10, 10, g.Width - 10, 40),
                font,
                StringAlignment.Far, StringAlignment.Near,
                timeleft.ToString(@"hh\:mm\:ss")
            );

        int y = 5 + this.Jump * (question.Text?.Length ?? 0) / this.Spacing
            + 500 - this.Jump * (question.Text?.Length ?? 0) / this.Spacing;

        if (question.Images != null && question.Images.Count > 0)
        {
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Near, StringAlignment.Near,
                "Pressione i para ver a imagem"
            );
            y += 40;
        }

        if (this.ShowImage && question.Images != null && question.Images.Count > 0)
        {
            g.DrawImage(
                new RectangleF(5, 5, g.Width - 10, g.Height - 10),
                question.Images[0]
            );
            return;
        }

        int index = 0;
        foreach (var alt in question.AlternativeTexts)
        {
            if (this.AlternativeSelected == index)
                g.FillRectangle(30, y, g.Width - 60, this.Jump * (alt.Length / this.Spacing + 1), Brushes.DarkGoldenrod);
            g.DrawText(
                new Rectangle(40, y, g.Width - 40, g.Height - y - 5),
                font, StringAlignment.Near, StringAlignment.Near,
                Context.AlternativesSelected.Keys.Any(a => a == Context.Current.ToString()) && Context.AlternativesSelected[Context.Current.ToString()] == index
                    ? Brushes.White
                    : this.AlternativeSelected == index
                        ? Brushes.Black
                        : Brushes.Black
                , alt
            );
            y += this.Jump * (alt.Length / this.Spacing + 1);
            index++;
        }
    }
}