using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
namespace RevitToCinima
{
    [Transaction(TransactionMode.Manual)]
    public class RevitToCinimaBinding : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            AppPenalRevitToCinima.ShowRevitToCinima();
            return Result.Succeeded;
        }
    }
}
