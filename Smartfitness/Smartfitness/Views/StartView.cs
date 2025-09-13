using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Pamella;


// App.Open<StartView>();
public class StartView : View
{
    protected override void OnStart(IGraphics g)
    {
        g.UnsubscribeKeyDownEvent(Context.KeyDownEvent);
        g.UnsubscribeKeyUpEvent(Context.KeyUpEvent);

        AlwaysInvalidateMode();

        bool hasQuestions = Context.Test.Questions.Count > 0;

        Action<Input> KDE = key =>
        {
        switch (key)
        {
            case Input.Space:
                {
                    Context.StartTime = TimeOnly.FromDateTime(DateTime.Now);
                    App.Push(new JumpView(new QuestionsView(), Input.Space, "INICIAR", Color.Black, Brushes.White, "Não foram encontradas questões", !hasQuestions));
                    break;
                }
            case Input.Escape:
                {
                    if (!hasQuestions)
                    {
                        App.Close();
                    }
                    break;
                }
            }
        };
        Context.KeyDownEvent = KDE;
        g.SubscribeKeyDownEvent(KDE);

        if (hasQuestions)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "../../Smartfitness/SmartfitinessLock.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process process = new Process { StartInfo = psi };
            process.Start();
        }
    }

    protected override void OnRender(IGraphics g)
    {
        g.Clear(Color.LightSteelBlue);

        g.DrawImage(
            new RectangleF(0, 0, g.Width, g.Height),
            Bitmap.FromFile("../../Smartfitness/Start.png")
        );

        g.DrawText(
                new Rectangle(5, 500, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Center, StringAlignment.Near,
                Brushes.White,
                "Pressione ESPAÇO para começar.."
            );
    }
}