using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class SchetsControl : UserControl
{   
    private Schets schets;
    private Color penkleur;

    public Color PenKleur
    { get { return penkleur; }
    }
    public Schets Schets
    { get { return schets;   }
    }
    public SchetsControl()
    {   this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
    private void teken(object o, PaintEventArgs pea)
    {   schets.Teken(pea.Graphics);
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {   schets.VeranderAfmeting(this.ClientSize);
        this.Invalidate();
    }
    public Graphics MaakBitmapGraphics()
    {   Graphics g = schets.BitmapGraphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        return g;
    }
    public void Schoon(object o, EventArgs ea)
    {   schets.Schoon();
        this.Invalidate();
    }
    public void Roteer(object o, EventArgs ea)
    {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Invalidate();
    }


    public void StelPenKleurIn(Color nieuweKleur)
    {
        penkleur = nieuweKleur;
    }


    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {   string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }

    private int penDikte = 3; // Standaard pen-dikte

    public int PenDikte
    {
        get { return penDikte; }
    }

    public void VeranderPenDikte(int nieuweDikte)
    {
        penDikte = nieuweDikte;
    }

    public void SlaOp(string bestandPad)
    {
        using (Bitmap bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height))
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Gebruik de bestaande tekenlogica
                this.schets.Teken(g);
            }

            // Sla de bitmap op in het gewenste formaat
            bitmap.Save(bestandPad);
        }
    }


}