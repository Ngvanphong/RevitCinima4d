using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace RevitToCinima
{
    public partial class frmRevitToCinima : System.Windows.Forms.Form
    {
        private ExternalEvent _eventRevit;
        private RevitToCinimaHandler _handlerRevit;
        public frmRevitToCinima(ExternalEvent eventRevit, RevitToCinimaHandler handlerRevit)
        {
            InitializeComponent();
            _eventRevit = eventRevit;
            _handlerRevit = handlerRevit;
        }

        private void frmRevitToCinima_Load(object sender, EventArgs e)
        {

        }

        private void btnExportCinima_Click(object sender, EventArgs e)
        {
            _eventRevit.Raise();
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Xml Files|*.xml";
            file.Multiselect = false;
            if (file.ShowDialog() == DialogResult.OK)
            {
                List<GroupElementRevit> listResult = new List<GroupElementRevit>();
                string fullPath = Path.GetFullPath(file.FileName);
                var xmlDoc = XDocument.Load(fullPath);
                var xmlElement = xmlDoc.Element("Table").Element("Element").Elements("Name");
                foreach (var item in xmlElement)
                {
                    string elementName = item.Element("NameElement").Value;
                    if (!listResult.Exists(x => x.NameElement == elementName))
                    {
                        var faceElements = item.Elements("Face");
                        bool faceFist = true;
                        foreach (var face in faceElements)
                        {
                            if (faceFist)
                            {
                                listResult.Add(CreateNewGorupFaceFirist(item, elementName));
                                faceFist = false;
                            }else
                            {
                                ModifiedOrAddFace(face, ref listResult, elementName);
                            }

                        }
                    }
                    else
                    {
                        var faceElements = item.Elements("Face");
                        foreach (var face in faceElements)
                        {
                            ModifiedOrAddFace(face, ref listResult, elementName);
                        }
                    }
                }

                ///Writer file again.
                WriteFileAgain(listResult);

            }

        }

        private void WriteFileAgain(List<GroupElementRevit> listResult)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Xml|*.xml";
            save.Title = "Save an Xml File";
            save.ShowDialog();
            if (save.FileName != "")
            {
                XmlTextWriter writer = new XmlTextWriter(save.FileName, System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Table");
                writer.WriteStartElement("Element");
                foreach (var item in listResult)
                {
                    writer.WriteStartElement("Name");

                    writer.WriteStartElement("NameElement");
                    writer.WriteString(item.NameElement);
                    writer.WriteEndElement();

                    foreach (var sub1 in item.ListMaterialPoint)
                    {
                        writer.WriteStartElement("Face");

                        writer.WriteStartElement("Material");
                        writer.WriteString(sub1.NameMaterial);
                        writer.WriteEndElement();

                        foreach (var sub2 in sub1.ListPoint)
                        {
                            writer.WriteStartElement("Point");
                            writer.WriteString(sub2);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }

        private GroupElementRevit CreateNewGorupFaceFirist(XElement item, string elementName)
        {
            GroupElementRevit newGoup = new GroupElementRevit();
            newGoup.NameElement = elementName;
            var faceElementFirst = item.Elements("Face").First();
            newGoup.ListMaterialPoint = new List<MaterialPoint>();
            string material = faceElementFirst.Element("Material").Value;
            MaterialPoint materialPoint = new MaterialPoint();
            materialPoint.NameMaterial = material;
            newGoup.ListMaterialPoint.Add(materialPoint);
            var pointElements = faceElementFirst.Elements("Point");
            materialPoint.ListPoint = new List<string>();
            foreach (var point in pointElements)
            {
                string pointValue = point.Value;
                materialPoint.ListPoint.Add(pointValue);
            }
            return newGoup;
        }

        private GroupElementRevit CreateNewFace(XElement face, string elementName,string material)
        {
            GroupElementRevit groupNew = new GroupElementRevit();
            groupNew.NameElement = elementName;
            groupNew.ListMaterialPoint = new List<MaterialPoint>();
            MaterialPoint materialPoint = new MaterialPoint();
            materialPoint.NameMaterial = material;
            groupNew.ListMaterialPoint.Add(materialPoint);
            var pointNews = face.Elements("Point");
            materialPoint.ListPoint = new List<string>();
            foreach (var point in pointNews)
            {
                string pointValue = point.Value;
                materialPoint.ListPoint.Add(pointValue);
            }
            return groupNew;
        }

        private void ModifiedOrAddFace(XElement face,ref List<GroupElementRevit> listResult,string elementName)
        {
            string materialValue = face.Element("Material").Value;
            foreach (GroupElementRevit goup in listResult)
            {
                string goupName = goup.NameElement;
                if (goupName == elementName)
                {
                    List<MaterialPoint> listMaterial = goup.ListMaterialPoint;
                    foreach (MaterialPoint materialP in listMaterial)
                    {
                        string materialName = materialP.NameMaterial;
                        if (materialName == materialValue)
                        {
                            var pointNews = face.Elements("Point");
                            foreach (var point in pointNews)
                            {
                                string pointValue = point.Value;
                                materialP.ListPoint.Add(pointValue);
                            }
                            break;
                        }
                        else
                        {
                           listResult.Add(CreateNewFace(face, elementName, materialName));
                            break;
                        }
                    }                  
                }
            }
        }


    }
    public class GroupElementRevit
    {
        public string NameElement { set; get; }

        public List<MaterialPoint> ListMaterialPoint { set; get; }

    }
    public class MaterialPoint
    {
        public string NameMaterial { set; get; }

        public List<string> ListPoint { set; get; }
    }
}
