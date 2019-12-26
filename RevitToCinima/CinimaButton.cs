using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using System.Windows.Media;
using Autodesk.Revit.UI;
namespace RevitToCinima
{
  public  class CinimaButton
    {
        public void RevitTo(UIControlledApplication application)
        {
            const string ribbonTag = "ArmoApiVn2";
            const string ribbonPanel = "Element2";
            try
            {
                application.CreateRibbonTab(ribbonTag);
            }
            catch { }
            RibbonPanel panel = null;
            List<RibbonPanel> panels = application.GetRibbonPanels(ribbonTag);
            foreach (RibbonPanel pl in panels)
            {
                if (pl.Name == ribbonPanel)
                {
                    panel = pl;
                    break;
                }
            }
            if (panel == null)
            {
                panel = application.CreateRibbonPanel(ribbonTag, ribbonPanel);
            }
            Image img = RevitToCinima.Properties.Resources.icons8_movie_32;
            ImageSource imgSrc = Helper.GetImageSource(img);
            PushButtonData btnData = new PushButtonData("RevitCinema", "RevitCinema",
                Assembly.GetExecutingAssembly().Location, "RevitToCinima.RevitToCinimaBinding")
            {
                ToolTip = "Export element from revit to cinima4d",
                LongDescription = "Export element from revit to cinima4d",
                Image = imgSrc,
                LargeImage = imgSrc,
            };

            PushButton button = panel.AddItem(btnData) as PushButton;
            button.Enabled = true;
        }
    }
}
