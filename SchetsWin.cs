using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class SchetsWin : Form
{   
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    Panel paneel;
    bool vast;

    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                      , this.ClientSize.Height - 50);
        paneel.Location = new Point(64, this.ClientSize.Height - 30);
    }

    private void klikToolMenu(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
    }

    private void klikToolButton(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
    }

    private void afsluiten(object obj, EventArgs ea)
    {
        this.Close();
    }

    public Size OrigineleGrootte { get; set; }

    public SchetsWin()
    {
        ISchetsTool[] deTools = { new PenTool()         
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new CirkelTool()
                                , new VolCirkelTool()
                                , new TekstTool()
                                , new GumTool()
                                };
        String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };

        this.ClientSize = new Size(900, 700);

        huidigeTool = deTools[0];

        schetscontrol = new SchetsControl();
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.StelPenKleurIn(Color.Black);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                    {   vast=true;  
                                        huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisLos (schetscontrol, mea.Location);
                                        vast = false; 
                                    };
        schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                    {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                    };
        this.Controls.Add(schetscontrol);

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }

    private void maakFileMenu()
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menu.DropDownItems.Add("Opslaan als", null, this.OpslaanAls);
        menuStrip.Items.Add(menu);
    }

    private void maakToolMenu(ICollection<ISchetsTool> tools)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {   ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
        ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
        foreach (string k in kleuren)
            submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
        menu.DropDownItems.Add(submenu);
        menuStrip.Items.Add(menu);
    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        int t = 0;
        foreach (ISchetsTool tool in tools)
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(45, 62);
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;
            this.Controls.Add(b);
            if (t == 0) b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren)
    {
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(600, 24);

        Button clear = new Button(); paneel.Controls.Add(clear);
        clear.Text = "Clear";
        clear.Location = new Point(0, 0);
        clear.Click += schetscontrol.Schoon;

        Button rotate = new Button(); paneel.Controls.Add(rotate);
        rotate.Text = "Rotate";
        rotate.Location = new Point(80, 0);
        rotate.Click += schetscontrol.Roteer;

        Button penkleur = new Button(); paneel.Controls.Add(penkleur);
        penkleur.Text = "Penkleur kiezen";
        penkleur.Size = new Size(100, 25);
        penkleur.Location = new Point(180, 0);
        penkleur.Click += MeerKleurenClick;

        Label pendikte = new Label(); paneel.Controls.Add(pendikte);
        pendikte.Location = new Point(370, 3);
        pendikte.AutoSize = true;

        TrackBar pend = new TrackBar(); paneel.Controls.Add(pend);
        pend.Location = new Point(500, 0);
        pend.Minimum = 2;
        pend.Maximum = 30;
        pend.ValueChanged += (sender, e) =>
        {
            int nieuwedikte = pend.Value;
            ((PenTool)huidigeTool).NieuweDikte(nieuwedikte);
            pendikte.Text = $"Pen Dikte: {nieuwedikte}";
        };
        pendikte.Text = $"Pen Dikte: {pend.Value}";

    }


    private void MeerKleurenClick(object sender, EventArgs e)
    {
        ColorDialog colorDialog = new ColorDialog();
        colorDialog.Color = schetscontrol.PenKleur;

        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            schetscontrol.StelPenKleurIn(colorDialog.Color); // Gebruik de methode om de kleur in te stellen
        }
    }

    private void OpslaanAls(object sender, EventArgs e)
    {
        SaveFileDialog saveDialog = new SaveFileDialog();
        saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
        saveDialog.Title = "Opslaan als";
        saveDialog.DefaultExt = "png";

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            this.schetscontrol.SlaOp(saveDialog.FileName);
        }
    }
}
