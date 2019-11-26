using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
namespace RevitToCinima
{
    public static class AppPenalRevitToCinima
    {
        public static frmRevitToCinima myFormRevitToCinima;
        public static void ShowRevitToCinima()
        {
            RevitToCinimaHandler handlerRevit = new RevitToCinimaHandler();
            ExternalEvent eventRevit = ExternalEvent.Create(handlerRevit);
            myFormRevitToCinima = new frmRevitToCinima(eventRevit, handlerRevit);
            myFormRevitToCinima.Show();
        }
    }
}
