using System;
using System.Drawing;
using Pamella;

    public class JumpView(
    View next,
    Input input,
    string text,
    Color? color = null,
    Brush? textBrush = null
) : View
{
    private bool Waiting = false;
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
                App.Push(next);
            if (key == input && !this.Go)
            {
                g.SubscribeKeyDownEvent(Context.KeyDownEvent);
                App.Pop();
            }
        };

        Context.KeyUpEvent = KUE;
        g.SubscribeKeyUpEvent(KUE);
    }
    protected override void OnRender(IGraphics g)
    {   
        g.Clear(color == null ? Color.White : (Color)color);

        bool shouldGo = (TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan() - this.Start).TotalSeconds > 2;
        if (shouldGo)
            this.Go = true;

        g.DrawText(
            new Rectangle(0, 0, g.Width, g.Height),
            new Font("Arial", 50), StringAlignment.Center, StringAlignment.Center,
            textBrush == null ? Brushes.Black : textBrush,
            shouldGo ? $"Solte a tecla para {text}" : $"Mantenha pressionado para {text}"
        );
    }
}