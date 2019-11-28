using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using Autodesk.Revit.DB;


namespace RevitToCinima
{
    public class RevitToCinimaHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            var floors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType().ToList();
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

                FloorXml(writer, floors,doc);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }

        public string GetName()
        {
            return "RevitToCinimar4dHandler";
        }

      
        public void FloorXml(XmlTextWriter writer,List<Element>listFloor,Document doc)
        {
            Options opt = new Options();
            writer.WriteStartElement("Floor");
            foreach(Element floor in listFloor)
            {
                
                writer.WriteStartElement("Name");
                writer.WriteString(floor.Name);
                try
                {
                    GeometryElement geo = floor.get_Geometry(opt);
                    foreach (GeometryObject obj in geo)
                    {
                        //GeometryInstance instance = obj as GeometryInstance;
                            //GeometryElement instanceGeometryElement = instance.GetSymbolGeometry();
                            foreach (GeometryObject instanceObj in geo)
                            {
                                Solid solid = instanceObj as Solid;
                                if (solid == null || 0 == solid.Faces.Size || solid.Edges.Size == 0)
                                {
                                    continue;
                                };
                                //Transaction insTransform = instance.Transform;
                                foreach (Face face in solid.Faces)
                                {
                                    writer.WriteStartElement("Face");
                                        string nameMaterial= doc.GetElement(face.MaterialElementId).Name;
                                        writer.WriteStartElement("Material");
                                        writer.WriteString(nameMaterial);
                                        writer.WriteEndElement();
                                        Mesh mesh = face.Triangulate();
                                        foreach (XYZ ii in mesh.Vertices)
                                        {
                                                writer.WriteStartElement("Point");
                                                 XYZ point = ii;
                                                writer.WriteString(point.ToString());
                                                 writer.WriteEndElement();                                  
                                        }
                                    writer.WriteEndElement();
                                }
                            }
                       
                    }
                }
                catch { continue; }


                writer.WriteEndElement();
            }
           
            writer.WriteEndElement();
        }

       
    }
}
