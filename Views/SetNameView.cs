using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Pamella;

App.Open<SetNameView>();
public class SetNameView     : View
{
    private string Name = "";
    private bool Flick = false;
    protected override async void OnStart(IGraphics g)
    {
        var crrPath = Environment.GetEnvironmentVariable("SENAI_EXAM")
            ?? throw new Exception("'SENAI_EXAM' is not defined");
        
        var files = Directory.GetFiles(crrPath);
        var file = files.FirstOrDefault(f =>
            Path.GetExtension(f) == ".exam"
        );
        
        if (file is null)
            return;
        var test = await Test.LoadFromExamFile(file)
            ?? throw new Exception("Test is null");

        Context.Test = test;
        
        AlwaysInvalidateMode();

        g.UnsubscribeKeyDownEvent(Context.KeyDownEvent);
        g.UnsubscribeKeyUpEvent(Context.KeyUpEvent);

        Action<Input> KDE = key =>
        {

            switch (key)
            {
                case Input.A: { this.Name += "a"; break; }
                case Input.B: { this.Name += "b"; break; }
                case Input.C: { this.Name += "c"; break; }
                case Input.D: { this.Name += "d"; break; }
                case Input.E: { this.Name += "e"; break; }
                case Input.F: { this.Name += "f"; break; }
                case Input.G: { this.Name += "g"; break; }
                case Input.H: { this.Name += "h"; break; }
                case Input.I: { this.Name += "i"; break; }
                case Input.J: { this.Name += "j"; break; }
                case Input.K: { this.Name += "k"; break; }
                case Input.L: { this.Name += "l"; break; }
                case Input.M: { this.Name += "m"; break; }
                case Input.N: { this.Name += "n"; break; }
                case Input.O: { this.Name += "o"; break; }
                case Input.P: { this.Name += "p"; break; }
                case Input.Q: { this.Name += "q"; break; }
                case Input.R: { this.Name += "r"; break; }
                case Input.S: { this.Name += "s"; break; }
                case Input.T: { this.Name += "t"; break; }
                case Input.U: { this.Name += "u"; break; }
                case Input.V: { this.Name += "v"; break; }
                case Input.W: { this.Name += "w"; break; }
                case Input.X: { this.Name += "x"; break; }
                case Input.Y: { this.Name += "y"; break; }
                case Input.Z: { this.Name += "z"; break; }
                case Input.Space: { this.Name += " "; break; }
                case Input.Back: { if (this.Name.Length > 0) this.Name = this.Name.Substring(0, this.Name.Length - 1); break; }
                case Input.Enter:
                    {
                        if (this.Name.Length > 0)
                        {
                            Context.Test.InstanceStudentName = this.Name;
                            App.Push(new StartView());
                        }
                        break;
                    }
            }
        };
        Context.KeyDownEvent = KDE;
        g.SubscribeKeyDownEvent(KDE);
    }

    protected override void OnRender(IGraphics g)
    {
        g.Clear(Color.Black);

        g.DrawText(
            new Rectangle(5, g.Height / 2 - 100, g.Width - 10, 40),
            new Font("Arial", 20), StringAlignment.Center, StringAlignment.Near,
            Brushes.White,
            "Digite seu nome para dar continuidade a prova:"
        );
        g.FillRectangle(10, g.Height / 2, g.Width - 10, 40, Brushes.LightGray);
        g.DrawText(
            new Rectangle(5, g.Height / 2, g.Width - 10, 40),
            new Font("Arial", 20), StringAlignment.Center, StringAlignment.Near,
            Brushes.Black,
            this.Name
        );
        g.DrawText(
            new Rectangle(g.Width/2 + (4 * this.Name.Length), g.Height / 2, g.Width - 10, 40),
            new Font("Arial", 20), StringAlignment.Near, StringAlignment.Near,
            Brushes.Black,
            this.Flick ? "_" : " "
        );
        this.Flick = !this.Flick;
    }
}