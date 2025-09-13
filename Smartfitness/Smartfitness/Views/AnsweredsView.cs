using System;
using System.Drawing;
using Pamella;

public class AnsweredsView : View
{
    protected override void OnStart(IGraphics g)
    {
        AlwaysInvalidateMode();

        Action<Input> KDE = key =>
        {
            switch (key)
            {
                case Input.Left:
                    {
                        Context.Current--;
                        App.Pop();
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


        g.DrawText(
            new Rectangle(5, 500, g.Width - 10, g.Height - 10),
            new Font("Arial", 20), StringAlignment.Center, StringAlignment.Near,
            Brushes.White,
            "Pressione F para terminar.."
        );
    }
}