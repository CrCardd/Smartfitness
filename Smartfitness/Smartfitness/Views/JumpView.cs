using System;
using System.Diagnostics;
using System.Drawing;
using Pamella;

    public class JumpView(
    View next,
    Input input,
    string text,
    Color? color = null,
    Brush? textBrush = null,
    string completeText = "",
    bool cancelNext = false
) : View
{
    private bool Go = false;
    private TimeSpan Start;
    protected override void OnStart(IGraphics g)
    {
        AlwaysInvalidateMode();

        g.UnsubscribeKeyDownEvent(Context.KeyDownEvent);
        g.UnsubscribeKeyUpEvent(Context.KeyUpEvent);

        this.Start = TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan();

        Action<Input> KUE = key =>
        {
            if (key == input && this.Go)
            {
                if (!cancelNext)
                    App.Push(next);
            }
            if (key == input && !this.Go)
            {
                g.SubscribeKeyDownEvent(Context.KeyDownEvent);
                App.Pop();
            }
            if (key == Input.Escape && cancelNext)
            {
                foreach (var proc in Process.GetProcessesByName("SmartfitinessLock"))
                    proc.Kill();
                App.Close();
            }
        };

        Context.KeyUpEvent = KUE;
        g.SubscribeKeyUpEvent(KUE);
    }
    protected override void OnRender(IGraphics g)
    {   
        g.Clear(color == null ? Context.BackgroundColor : (Color)color);

        bool shouldGo = (TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan() - this.Start).TotalSeconds > 2;
        if (shouldGo)
            this.Go = true;

        g.DrawText(
            new Rectangle(0, 0, g.Width, g.Height),
            new Font("Arial", 50), StringAlignment.Center, StringAlignment.Center,
            textBrush == null ? Brushes.Black : textBrush,
            cancelNext ? completeText : (shouldGo ? $"Solte a tecla para {text}" : $"Mantenha pressionado para {text}")
        );
    }
}