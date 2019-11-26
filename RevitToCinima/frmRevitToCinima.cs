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
namespace RevitToCinima
{
    public partial class frmRevitToCinima : Form
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
    }
}
