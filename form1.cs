using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Net;
using System.IO;




namespace Samer_Import_Export
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML files (*.xml)|*.xml";

            label1.Text = "";
            //Open Dialogbox to Open XML File
            openFileDialog1.ShowDialog();

            //Declare the selected file as working CML
            Globals.fileNameText = openFileDialog1.FileName;
            label1.Text = Globals.fileNameText;

            //Creating XMLDocument object 
            //XmlDocument _doc = new XmlDocument();

            //Loading XMLDocument object
            //_doc.Load(Globals.fileNameText);


            var _doc = XDocument.Load(Globals.fileNameText);

            foreach (var childElem in _doc.Elements("CargoManifestMessage"))
            {
                foreach (var child in childElem.Elements("Header"))
                {
                    Globals.Headers.Clear();

                    foreach(var x in child.Elements())
                    {
                        
                        Globals.Headers.Add(x.Name.ToString(), x.Value.ToString());
                    }
                    txtSender.Text = child.Element("sender").Value ;
                    txtReciever.Text = child.Element("reciever").Value;
                    txtMessage.Text = child.Element("typeOfMessage").Value;
                    txtDateTime.Text = child.Element("timeOfMessage").Value;
                }

                
                foreach (var child in childElem.Elements("CargoManifestData"))
                {
                    foreach(var x in child.Elements())
                    {
                        var checkName = x.Name.ToString();

                        switch (checkName)
                        {
                            case "Vessel":
                                Globals.Vessels.Clear();
                                foreach (var y in x.Elements())
                                {
                                    Globals.Vessels.Add(y.Name.ToString(), y.Value.ToString());
                                }
                                break;
                            case "Voyage":
                                Globals.Voyages.Clear();
                                foreach (var y in x.Elements())
                                {
                                    Globals.Voyages.Add(y.Name.ToString(), y.Value.ToString());
                                }
                                break;
                            case "BLs":
                                int recs = 0;

                                Globals.BL.Clear();
                                textBox2.Clear();

                                foreach (var y in x.Elements())
                                {
                                    
                                    
                                       foreach (var z in x.Elements("units"))
                                       {
                                           Globals.Units.Add(y.Element("number").Value.ToString(), z.Value.ToString());

                                       }

                                    Globals.BL.Add(y.Element("number").Value.ToString(), y.Value.ToString());

                                    recs = recs + 1;
                                    textBox2.AppendText("\n" + y.ToString());


                                    string output = string.Join("\n", Globals.BL);
                                    MessageBox.Show(output);
                                    

                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                //textBox2.AppendText(Globals.BL.ToString());


                Globals.data.Clear();
                foreach (var x in Globals.Headers)
                {
                    Globals.data.Add(x.Key, x.Value);
                    //Globals.tmpHeadKey = Globals.tmpHeadKey + "," + headerVal.Key;
                    //Globals.tmpHeadVal = Globals.tmpHeadVal + "," + headerVal.Value;
                    //dataGridView1.Columns[dg].Name = headerVal.Key;
                }
                foreach (var x in Globals.Vessels)
                {
                    Globals.data.Add(x.Key, x.Value);
    
                }
                foreach (var x in Globals.Voyages)
                {
                    Globals.data.Add(x.Key, x.Value);
                }
                
                var dataCount = Globals.data.Keys.Count - 1;
                
                textBox1.AppendText("Insert into table ('");
                foreach (var _key in Globals.data.Keys)
                {
                    if (_key == Globals.data.Keys.Last())
                    {
                        textBox1.AppendText(_key + "'");
                    }
                    else {
                        textBox1.AppendText(_key + "','");
                    }
                }
                textBox1.AppendText(") values ('");
                foreach (var _val in Globals.data.Values)
                {
                    if (_val == Globals.data.Values.Last())
                    {
                        textBox1.AppendText(_val + "'");
                    }
                    else
                    {
                        textBox1.AppendText(_val + "','");
                    }
                }
                textBox1.AppendText("');");
            }
        }


        public static class Globals
        {
            public static bool MyStaticBool { get; set; }

            public static string tmpHeadKey { get; set; }
            public static string tmpHeadVal { get; set; }
            public static string fileNameText { get; set; }

            public static IDictionary<string, string> Headers = new Dictionary<string, string>();
            public static IDictionary<string, string> Vessels = new Dictionary<string, string>();
            public static IDictionary<string, string> Voyages = new Dictionary<string, string>();
            public static IDictionary<string, string> BLs = new Dictionary<string, string>();
            public static IDictionary<string, string> BL = new Dictionary<string, string>();

            public static IDictionary<string, string> Units = new Dictionary<string, string>();
            public static IDictionary<string, string> Goods = new Dictionary<string, string>();
            public static IDictionary<string, string> DangerousGoods = new Dictionary<string, string>();
            public static IDictionary<string, string> Passengers = new Dictionary<string, string>();

            public static IDictionary<string, string> data = new Dictionary<string, string>();
            public static IDictionary<string, string> bldata = new Dictionary<string, string>(); 


            public static string[] row ;
          
        }

        private void btFillGrid_Click_1(object sender, EventArgs e)
        {
            if(label1.Text != "")
            {

                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

            dataGridView1.ColumnCount = Globals.Headers.Count + Globals.Vessels.Count + Globals.Voyages.Count;

            int cols = 0;
            foreach (var y in Globals.data)
            {
                dataGridView1.Columns[cols].Name = y.Key.ToString();
                cols = cols + 1;
            }
            dataGridView1.Rows.Add(Globals.data.Values.ToArray());
            //this.dataGridView1.DataSource = Globals.data.Values.ToList();
        
            }
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();

            dataGridView2.ColumnCount = Globals.data.Count;
            int colx = 0;
            foreach (var x in Globals.BL)
            {
                dataGridView2.Rows.Add(Globals.BL.Values.ToArray());
                //dataGridView2.Columns[colx].Name = x.Key.ToString();
                colx = colx + 1;
            }
            //dataGridView2.Rows.Add(Globals.BL.Values.ToArray());
        }
   
    }

}


