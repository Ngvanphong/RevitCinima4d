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
            var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToList();
            List<Element> allElements = new List<Element>();
            foreach (Element e in collector)
            {
                if (e.Category != null && e.Category.HasMaterialQuantities)
                {
                    allElements.Add(e);
                }
            }
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

                CreateDataXml(writer, allElements, doc);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }

        public string GetName()
        {
            return "RevitToCinimar4dHandler";
        }


        public void CreateDataXml(XmlTextWriter writer, List<Element> listElements, Document doc)
        {
            Options opt = new Options();
            writer.WriteStartElement("Element");
            foreach (Element element in listElements)
            {
                writer.WriteStartElement("Name");
                writer.WriteString(element.Name);
                try
                {
                    GeometryElement geo = element.get_Geometry(opt);
                    foreach (GeometryObject obj in geo)
                    {
                        GeometryInstance instance = obj as GeometryInstance;
                        if (instance != null)
                        {
                            GeometryElement instanceGeometryElement = instance.GetSymbolGeometry();
                            foreach (GeometryObject instanceObj in instanceGeometryElement)
                            {
                                Solid solid = instanceObj as Solid;
                                if (solid == null || 0 == solid.Faces.Size || solid.Edges.Size == 0)
                                {
                                    continue;
                                };
                                foreach (Face face in solid.Faces)
                                {
                                    writer.WriteStartElement("Face");
                                    string nameMaterial = doc.GetElement(face.MaterialElementId).Name;
                                    writer.WriteStartElement("Material");
                                    writer.WriteString(nameMaterial);
                                    writer.WriteEndElement();
                                    Mesh mesh = face.Triangulate();
                                    for (int i = 0; i < mesh.NumTriangles; i++)
                                    {
                                        MeshTriangle triangle = mesh.get_Triangle(i);
                                        for (int g = 0; g < 3; g++)
                                        {
                                            writer.WriteStartElement("Point");
                                            XYZ point = triangle.get_Vertex(g);
                                            writer.WriteString(point.ToString());
                                            writer.WriteEndElement();
                                        }
                                    };
                                    writer.WriteEndElement();
                                }
                            }
                        }
                        else
                        {
                            Solid solid = obj as Solid;
                            if (solid == null || 0 == solid.Faces.Size || solid.Edges.Size == 0)
                            {
                                continue;
                            };                       
                            foreach (Face face in solid.Faces)
                            {
                                writer.WriteStartElement("Face");
                                string nameMaterial = doc.GetElement(face.MaterialElementId).Name;
                                writer.WriteStartElement("Material");
                                writer.WriteString(nameMaterial);
                                writer.WriteEndElement();
                                Mesh mesh = face.Triangulate();
                                for (int i = 0; i < mesh.NumTriangles; i++)
                                {
                                    MeshTriangle triangle = mesh.get_Triangle(i);
                                    for (int g = 0; g < 3; g++)
                                    {
                                        writer.WriteStartElement("Point");
                                        XYZ point = triangle.get_Vertex(g);
                                        writer.WriteString(point.ToString());
                                        writer.WriteEndElement();
                                    }
                                };
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
