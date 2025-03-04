using System;
using System.Drawing;
using System.Drawing.Drawing2D;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);
}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 40);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }
    }
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {
        return new Rectangle(
            new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y)),
            new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
        );
    }

    public static Pen MaakPen(Brush b, int dikte)
    {
        Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }

    public override void MuisVast(SchetsControl s, Point p)
    {
        base.MuisVast(s, p);
        kwast = new SolidBrush(s.PenKleur);
    }

    public override void MuisDrag(SchetsControl s, Point p)
    {
        s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }

    public override void MuisLos(SchetsControl s, Point p)
    {
        base.MuisLos(s, p);
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
        s.Invalidate();
    }

    public override void Letter(SchetsControl s, char c)
    {
        // Niet nodig voor TweepuntTool
    }

    public abstract void Bezig(Graphics g, Point p1, Point p2);

    public virtual void Compleet(Graphics g, Point p1, Point p2)
    {
        this.Bezig(g, p1, p2);
    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}
    
public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {   g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}

public class CirkelTool : TweepuntTool
{
    public override string ToString() { return "cirkel"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawEllipse(MaakPen(kwast, 3), Punten2Rechthoek(p1, p2));
    }
}


public class VolCirkelTool : TweepuntTool
{
    public override string ToString() { return "plaat"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.FillEllipse(kwast, Punten2Rechthoek(p1, p2));
    }
}



public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(this.kwast,3), p1, p2);
    }
}

public class PenTool : LijnTool
{
    private int penDikte = 2;

    public override string ToString() { return "pen"; }

    public override void MuisDrag(SchetsControl s, Point p)
    {
        this.MuisLos(s, p);
        this.MuisVast(s, p);
    }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        Pen pen = MaakPen(this.kwast, penDikte);
        g.DrawLine(pen, p1, p2);
    }

    public void NieuweDikte(int dikte2)
    {
        penDikte = dikte2;
    }
}

public class GumTool : PenTool
{
    public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
    }
}