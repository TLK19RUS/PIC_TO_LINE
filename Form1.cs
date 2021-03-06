using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PIC_TO_LINE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public struct line
        {
            public int y;
            public int x_start;
            public int x_end;
            public ushort color;
        }

        public ushort rgb888_to_rgb565(Color color)
        {
            return (ushort)(((color.R & 0b11111000) << 8) | ((color.G & 0b11111100) << 3) | (color.B >> 3));
        }

        public String int_to_hex(byte i)
        {
            return "0x" + Convert.ToString(i, 16).ToUpper().PadLeft(2,'0');
        }

        public String int_to_hex(int i)
        {
            return "0x" + Convert.ToString(i, 16).ToUpper().PadLeft(2, '0');
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int line_start=0;
            Color line_color = Color.Black;
            Color cur_color;
            List<line> lines = new List<line>();
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = Environment.CurrentDirectory;
            //if (ofd.ShowDialog() == DialogResult.OK)
            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\out", false, System.Text.Encoding.Default);
            for (int f=0;f<10;f++)
            {
                sw.WriteLine("const uint8_t digit_"+f.ToString()+"[]{");
                lines.Clear();
                Bitmap b = new Bitmap(Environment.CurrentDirectory + "\\" + f.ToString() + ".png");
                textBox1.Text += b.Width.ToString() + "\r\n";
                textBox1.Text += b.Height.ToString() + "\r\n";

                for(int y = 0; y < b.Height; y++)
                {
                    for (int x = 0; x < b.Width; x++)
                    {
                        cur_color = b.GetPixel(x, y);
                        if (x == 0)
                        {
                            line_start = x;
                            line_color = cur_color;
                        }
                        if (y == 29 && x == 31)
                        {
                            y = 29;
                        }
                        if ((line_color != cur_color) || (x==b.Width-1))
                        {
                            line nl = new line();
                            nl.y = y;
                            nl.x_start = line_start;
                            
                            if (x == b.Width - 1)
                            {
                                if (line_color == cur_color)
                                {
                                    nl.x_end = x;
                                }
                                else
                                {
                                    nl.x_end = x - 1;
                                }
                            }
                            else {
                                nl.x_end = x - 1;
                            }
                            
                            nl.color = rgb888_to_rgb565(line_color);
                            lines.Add(nl);

                            if ((line_color != cur_color) && (x == b.Width - 1))
                            {
                                nl.x_start = x;
                                nl.x_end = x;
                                nl.color = rgb888_to_rgb565(cur_color);
                                lines.Add(nl);
                            }


                            line_start = x;
                            line_color = cur_color;
                        }
                        //textBox1.Text += b.GetPixel(x,y).R.ToString() + "," + b.GetPixel(x, y).G.ToString() + "," + b.GetPixel(x, y).B.ToString()+ "\r\n";
                    }
                }
                //sw.WriteLine("// " + f.ToString());
                //textBox1.Text += "// " + f.ToString() + "\r\n";
                for (int i=0;i<lines.Count;i++)
                {
                    String tmp;
                    if (i==lines.Count - 1 && f==9)
                    {
                        tmp = "";
                    }
                    else
                    {
                        tmp = ",";
                    }
                    sw.WriteLine(int_to_hex(lines[i].y) + "," + int_to_hex(lines[i].x_start) + "," + int_to_hex(lines[i].x_end) + "," + int_to_hex((byte)(lines[i].color >> 8)) + "," + int_to_hex((byte)(lines[i].color & 0xFF)) + tmp);
                    //textBox1.Text += int_to_hex(lines[i].y) + ","+ int_to_hex(lines[i].x_start) + "," + int_to_hex(lines[i].x_end) + "," + int_to_hex((byte)(lines[i].color >> 8)) + "," + int_to_hex((byte)(lines[i].color & 0xFF)) + tmp + "\r\n";

                }
                sw.WriteLine("};");
            }
            sw.Flush();
            sw.Close();
        }
    }
}
