using System;
using System.Drawing;
using System.Windows.Forms;

public class SchetsEditor : Form
{
    private MenuStrip menuStrip;

    public SchetsEditor()
    {   
        this.ClientSize = new Size(1100, 900);
        menuStrip = new MenuStrip();
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakHelpMenu();
        this.Text = "Schets editor";
        this.IsMdiContainer = true;
        this.MainMenuStrip = menuStrip;
        this.Resize += this.AanpassenChildWindows;
    }
    private void AanpassenChildWindows(object sender, EventArgs e)
    {
        foreach (Form child in this.MdiChildren)
        {
            if (child is SchetsWin schetsWin)
            {
                // Bereken de schaalfactor op basis van de originele grootte
                float widthRatio = (float)this.ClientSize.Width / 1100;
                float heightRatio = (float)this.ClientSize.Height / 900;

                int newWidth = (int)(schetsWin.OrigineleGrootte.Width * widthRatio);
                int newHeight = (int)(schetsWin.OrigineleGrootte.Height * heightRatio);

                // Stel de nieuwe grootte in
                schetsWin.ClientSize = new Size(newWidth, newHeight);
            }
        }
    }



    private void maakFileMenu()
    {   
        ToolStripDropDownItem menu = new ToolStripMenuItem("File");
        menu.DropDownItems.Add("Nieuw", null, this.nieuw);
        menu.DropDownItems.Add("Exit", null, this.afsluiten);
        menuStrip.Items.Add(menu);
    }
    private void maakHelpMenu()
    {   
        ToolStripDropDownItem menu = new ToolStripMenuItem("Help");
        menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
        menuStrip.Items.Add(menu);
    }
    private void about(object o, EventArgs ea)
    {   
        MessageBox.Show ( "Schets versie 2.0\n(c) UU Informatica 2024"
                        , "Over \"Schets\""
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Information
                        );
    }

    private void nieuw(object sender, EventArgs e)
{   
    SchetsWin s = new SchetsWin();
    s.MdiParent = this;
    s.OrigineleGrootte = s.ClientSize; // Sla de oorspronkelijke grootte op
    s.Show();
}

    private void afsluiten(object sender, EventArgs e)
    {   
        this.Close();
    }

}