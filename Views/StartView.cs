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
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "./SmartfitinessLock.exe",      
            UseShellExecute = false,     
            CreateNoWindow = true,       
            WindowStyle = ProcessWindowStyle.Hidden
        };
        Process process = new Process { StartInfo = psi };
        process.Start();
        
        
        g.UnsubscribeKeyDownEvent(Context.KeyDownEvent);
        g.UnsubscribeKeyUpEvent(Context.KeyUpEvent);

        AlwaysInvalidateMode();
        Action<Input> KDE = key =>
        {
            switch (key)
            {
                case Input.Space:
                    {
                        Context.StartTime = TimeOnly.FromDateTime(DateTime.Now);
                        App.Push(new JumpView(new QuestionsView(), Input.Space, "INICIAR", Color.Black, Brushes.White));
                        break;
                    }
            }
        };
        Context.KeyDownEvent = KDE;
        g.SubscribeKeyDownEvent(KDE);
    }

    protected override void OnRender(IGraphics g)
    {
        g.Clear(Color.LightSteelBlue);

        g.DrawImage(
            new RectangleF(0, 0, g.Width, g.Height),
            Bitmap.FromFile("./Start.png")
        );
        
        g.DrawText(
                new Rectangle(5, 500, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Center, StringAlignment.Near,
                Brushes.White,
                "Pressione ESPAÇO para começar.."
            );
    }
}