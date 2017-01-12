using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ScintillaNET;

namespace XslMappingApp
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();


            this.xslString.Margins[0].Width = 25;
          
            this.xslString.Lexer = ScintillaNET.Lexer.Xml;
            // Instruct the lexer to calculate folding
            xslString.SetProperty("fold", "1");
            xslString.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            xslString.Margins[2].Type = MarginType.Symbol;
            xslString.Margins[2].Mask = Marker.MaskFolders;
            xslString.Margins[2].Sensitive = true;
            xslString.Margins[2].Width = 20;
            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                xslString.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                xslString.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            //xslString.Styles[Style.Xml.Default].ForeColor = Color.Silver;
            xslString.Styles[Style.Xml.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            xslString.Styles[Style.Xml.CData].ForeColor = Color.FromArgb(0, 128, 0); // Green
          
            xslString.Styles[Style.Xml.Number].ForeColor = Color.Olive;
            xslString.Styles[Style.Xml.Tag].ForeColor = Color.Blue;
            xslString.Styles[Style.Xml.TagEnd].ForeColor = Color.Blue;
            xslString.Styles[Style.Xml.XmlStart].ForeColor = Color.FromArgb(163, 21, 21); // Red
            xslString.Styles[Style.Xml.XmlEnd].ForeColor = Color.FromArgb(163, 21, 21); // Red
            xslString.Styles[Style.Xml.Attribute].ForeColor = Color.FromArgb(163, 21, 21); // Red
            xslString.Styles[Style.Xml.Entity].BackColor = Color.Pink;
            //xslString.Styles[Style.Xml.SingleString].ForeColor = Color.Purple;
            //xslString.Styles[Style.Xml.Value].ForeColor = Color.Maroon;

            // Configure folding markers with respective symbols
            xslString.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            xslString.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            xslString.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            xslString.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            xslString.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            xslString.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            xslString.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            xslString.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);






            this.sourceXmlString.AllowDrop = true;
            this.sourceXmlString.DragEnter += new DragEventHandler((s,e)=> {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            });
            this.sourceXmlString.DragDrop += new DragEventHandler((s,e)=> {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);
                    this.sourceXmlString.Text = Beautify(doc);
                }
                
            });


            this.xslString.AllowDrop = true;
            this.xslString.DragEnter += new DragEventHandler((s, e) => {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            });
            this.xslString.DragDrop += new DragEventHandler((s, e) => {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);
                    this.xslString.Text = Beautify(doc);
                }

            });


        }

        private void sourcePath_TextChanged(object sender, EventArgs e)
        {

        }

        public  string Beautify( XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        private void sourcePath_DoubleClick(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(myStream);
                            this.sourceXmlString.Text = Beautify(doc);


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }


        public void Trans() {

            XslCompiledTransform xsl = new XslCompiledTransform();
            XmlReader xslReader = XmlReader.Create(new StringReader(this.xslString.Text));
            //xsl.Load(xslReader);
            XmlReader input = XmlReader.Create(new StringReader(this.sourceXmlString.Text));
            XsltSettings setting = new XsltSettings(true, true);
            setting.EnableDocumentFunction = true;
            setting.EnableScript = true;
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings wset = new XmlWriterSettings();
            wset.Indent = true;
            wset.IndentChars = "  ";
            wset.NewLineChars = "\r\n";
            wset.NewLineHandling = NewLineHandling.Replace;
            wset.OmitXmlDeclaration = false;
            wset.ConformanceLevel = ConformanceLevel.Fragment;
            //XmlWriter output = XmlWriter.Create(sb, wset);
            StringWriter tw = new StringWriter(sb);
            XmlTextWriter output = new XmlTextWriter(tw);
            //StreamWriter sw = new StreamWriter("c:\\out.xml");
            //XmlWriter output = XmlWriter.Create(sw, wset);
            XsltArgumentList arg = new XsltArgumentList();
            xsl.Load(xslReader, setting, null);
            xsl.Transform(input,null, output);
            //output.Flush();
            //var len = sw.BaseStream.Length;
            //output.Close();
            //sw.Close();
            if (sb.ToString().StartsWith("<"))
            {
                this.toXmlString.Text = PrettyXml(sb.ToString());
            }
            else
            {
                this.toXmlString.Text = sb.ToString();
            }
        }
         string PrettyXml(string xml)
        {
            var stringBuilder = new StringBuilder();

            var element = XElement.Parse(xml);

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }
        private void xslPath_DoubleClick(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "xsl files (*.xls)|*.xls|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(myStream);
                            this.xslString.Text = Beautify(doc);


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void toXmlString_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.Trans();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mapping error: " + ex.Message);
            }
        }

        private void xslString_DoubleClick(object sender, ScintillaNET.DoubleClickEventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "xsl files (*.xls)|*.xls|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(myStream);
                            this.xslString.Text = Beautify(doc);


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
