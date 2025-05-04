using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastChartingCore;

namespace FastCharting
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        int circles = 0;
        string b = string.Empty, x = string.Empty, time = string.Empty;
        bool h = false;
        string slide = string.Empty;
        string HSlide = string.Empty;
        bool breakslide = false;


        private void txtCircles_TextChanged(object sender, EventArgs e)
        {
            circles = int.TryParse(txtCircles.Text, out circles) ? circles : 0;
        }


        private void chkB_CheckedChanged(object sender, EventArgs e)
        {
            txtB.Enabled = chkB.Checked;
            if (chkB.Checked) b = txtB.Text;
        }
        private void txtB_TextChanged(object sender, EventArgs e)
        {
            b = txtB.Text;
        }


        private void chkX_CheckedChanged(object sender, EventArgs e)
        {
            txtX.Enabled = chkX.Checked;
            if (chkX.Checked) x = txtX.Text;
        }
        private void txtX_TextChanged(object sender, EventArgs e)
        {
            x = txtX.Text;
        }


        private void chkH_CheckedChanged(object sender, EventArgs e)
        {
            txtTime.Enabled = chkH.Checked;
            chkSlide.Enabled = !chkH.Checked;

            txtHSlide.Enabled = chkH.Checked;
            h = chkH.Checked;
            if (chkH.Checked) time = txtTime.Text;
            HSlide = txtHSlide.Text;
        }
        private void chkSlide_CheckedChanged(object sender, EventArgs e)
        {
            txtSlide.Enabled = chkSlide.Checked;
            txtTime.Enabled = chkSlide.Checked;
            chkH.Enabled = !chkSlide.Checked;

            txtHSlide.Enabled = chkSlide.Checked;
            if (chkSlide.Checked)
            {
                slide = txtSlide.Text;
                time = txtTime.Text;
            }
            breakslide = chkBreakSlide.Checked;
            HSlide = txtHSlide.Text;
        }
        private void txtSlide_TextChanged(object sender, EventArgs e)
        {
            slide = txtSlide.Text;
        }
        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            time = txtTime.Text;
        }

        private void chkBreakSlide_CheckedChanged(object sender, EventArgs e)
        {
            breakslide = chkBreakSlide.Checked;
        }

        private void lbSpinHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmSpinHelp frmSpinHelp = new FrmSpinHelp();
            frmSpinHelp.ShowDialog();
        }

        private void btnOutToIn_Click(object sender, EventArgs e)
        {
            txtSlideTemplate.Text = txtOut.Text;
        }

        private void txtAutoFillArgs_TextChanged(object sender, EventArgs e)
        {
            txtBPM.Text = Regex.Match(txtAutoFillArgs.Text, @"\([^)]+\)").Value.Trim('(').Trim(')');
            txtSlideInterval.Text = Regex.Match(txtAutoFillArgs.Text, @"\{[^}]+\}").Value.Trim('{').Trim('}') + ":1";
        }

        private void txtHSlide_TextChanged(object sender, EventArgs e)
        {
            HSlide = txtHSlide.Text;
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            switch (tabGenMode.SelectedIndex)
            {
                case 0:
                    if (!chkB.Checked) b = string.Empty;
                    if (!chkX.Checked) x = string.Empty;
                    if (!chkSlide.Checked) slide = string.Empty;
                    if (!(chkSlide.Checked || chkH.Checked))
                    {
                        HSlide = string.Empty;
                        time = string.Empty;
                    }
                    txtOut.Text = Gen.GetSpin(circles, b, x, h, slide, HSlide, time, breakslide);
                    break;
                case 1:
                    string[] slides = txtSlideTemplate.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (cboxUnifyMode.SelectedIndex == 0)
                    {
                        var times = Gen.UnifyBeginTime(double.Parse(txtBPM.Text), txtSlideBeginTime.Text, slides.Length, txtSlideInterval.Text, txtSlideTime.Text);
                        for (int i = 0; i < slides.Length; i++)
                        {
                            slides[i] = Regex.Replace(slides[i], @"\[[^\]]+\]", "");
                            slides[i] += times[i];
                        }
                        txtOut.Text = string.Join(",", slides) + ",";
                    }
                    else if (cboxUnifyMode.SelectedIndex == 1)
                    {
                        var times = Gen.UnifyEndTime(double.Parse(txtBPM.Text), txtSlideBeginTime.Text, slides.Length, txtSlideInterval.Text, txtSlideTime.Text);
                        for (int i = 0; i < slides.Length; i++)
                        {
                            slides[i] = Regex.Replace(slides[i], @"\[[^\]]+\]", "");
                            slides[i] += times[i];
                        }
                        txtOut.Text = string.Join(",", slides) + ",";
                    }
                    break;
                case 2:
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < int.Parse(txtRepeatNumber.Text); i++)
                    {
                        sb.Append(txtRepeatContent.Text);
                    }
                    txtOut.Text = sb.ToString();
                    break;
            }
        }
    }
}
