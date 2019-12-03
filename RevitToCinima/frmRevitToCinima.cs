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
                foreach(var item in xmlElement)
                {
                    string elementName = item.Element("NameElement").Value;
                    if (!listResult.Exists(x => x.NameElement == elementName))
                    {
                        GroupElementRevit newGoup = new GroupElementRevit();
                        newGoup.NameElement = elementName;
                        listResult.Add(newGoup);
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

        public List<XYZ> ListPoint { set; get; }
    }
}
