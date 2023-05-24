using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMg
{
    public partial class SetForm : Form
    {
        Label FontSizeLabel;
        Label FontStyleLabel;
        Label IconSize;
        Label BackColorLabel;
        NumericUpDown ListOfSizes;
        ComboBox ListOfFonts;
        Button OkButton;

        Font SettingsFont;
        int LabelHeight = 20;
        ComboBox comboBoxFont;
        Form1 ParentForm;
        ListView listView;

        User CurrentUser;

        public SetForm(ListView listV, Form1 formMain, User currentUser) //Form1 formMain
        {
            InitializeComponent();
            CurrentUser = currentUser;
            ParentForm = formMain;
            listView = listV;

            Text = "Настройки";
            Size = new Size(400, 320);
            SettingsFont = new Font("Arial", 12);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            FormClosed += FormSettings_FormClosed;
            

            CenterToParent();

            FontSizeLabel = new Label();
            FontSizeLabel.Text = "Размер шрифта:";
            FontSizeLabel.Location = new Point(20, 30);
            FontSizeLabel.Size = new Size(150, LabelHeight);
            FontSizeLabel.Font = SettingsFont;
            Controls.Add(FontSizeLabel);

            ListOfSizes = new NumericUpDown();
            ListOfSizes.Location = new Point(180, 30);
            ListOfSizes.Size = new Size(180, LabelHeight);
            ListOfSizes.Font = SettingsFont;
            ListOfSizes.Maximum = 20;
            ListOfSizes.Minimum = 8;
            ListOfSizes.Value = CurrentUser.FontSize;
            Controls.Add(ListOfSizes);

            FontStyleLabel = new Label();
            FontStyleLabel.Text = "Стиль шрифта:";
            FontStyleLabel.Location = new Point(20, 90);
            FontStyleLabel.Size = new Size(150, LabelHeight);
            FontStyleLabel.Font = SettingsFont;
            Controls.Add(FontStyleLabel);

            ListOfFonts = new ComboBox();
            ListOfFonts.Location = new Point(180, 90);
            ListOfFonts.Size = new Size(180, LabelHeight);
            ListOfFonts.Font = SettingsFont;
            foreach (FontFamily fontFamily in FontFamily.Families)
            {
                ListOfFonts.Items.Add(fontFamily.Name);
            }
            ListOfFonts.SelectedItem = CurrentUser.FontFamily;
            Controls.Add(ListOfFonts);

            BackColorLabel = new Label();
            BackColorLabel.Text = "Цвет фона";
            BackColorLabel.Location = new Point(20, 150);
            BackColorLabel.Size = new Size(150, LabelHeight);
            BackColorLabel.Font = SettingsFont;
            Controls.Add(BackColorLabel);

            Color[] colors = { Color.Beige, Color.Tomato, Color.Gold, Color.Moccasin, Color.Pink,
                               Color.RoyalBlue, Color.YellowGreen, Color.LightCyan, Color.Gainsboro, Color.White };
            for (int i = 0; i < 10; i++)
            {
                Panel panel = new Panel();
                panel.BackColor = colors[i];
                panel.BorderStyle = BorderStyle.FixedSingle;
                panel.Size = new Size(30, 30);
                panel.Location = new Point(180 + 30 * (i % 5), 150 + 30 * (i / 5));
                panel.Click += Panel_Click;
                Controls.Add(panel);
            }

            OkButton = new Button();
            OkButton.Text = "OK";
            OkButton.Location = new Point(150, 230);
            OkButton.Size = new Size(80, 2 * LabelHeight);
            OkButton.Font = SettingsFont;
            OkButton.BackColor = Color.White;
            OkButton.Click += OkButton_Click;
            Controls.Add(OkButton);

            comboBoxFont = new ComboBox();
            foreach (FontFamily fontFamily in FontFamily.Families)
            {
                comboBoxFont.Items.Add(fontFamily.Name);
            }
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            ParentForm.ChangeAppearance((int)ListOfSizes.Value, ListOfFonts.SelectedItem.ToString(), CurrentUser.BackgroundColor);
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                listView.BackColor = panel.BackColor;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            CurrentUser.FontSize = ListOfSizes.Value;
            CurrentUser.FontFamily = ListOfFonts.SelectedItem.ToString();
            CurrentUser.BackgroundColor = listView.BackColor;
            ParentForm.ChangeAppearance((int)ListOfSizes.Value, ListOfFonts.SelectedItem.ToString(), CurrentUser.BackgroundColor);
            Close();
        }
    

        private void SetForm_Load(object sender, EventArgs e)
        {

        }
    }
}
