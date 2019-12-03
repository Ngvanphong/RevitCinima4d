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
using System.Transactions;

namespace RevitToCinima
{
    public class RevitToCinimaHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToList();
            var collectSelection = app.ActiveUIDocument.Selection.GetElementIds();
            List<Element> allElements = new List<Element>();
            foreach (ElementId id in collectSelection)
            {
                Element el = doc.GetElement(id);
                allElements.Add(el);
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
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;
            writer.WriteStartElement("Element");
            foreach (Element element in listElements)
            {
                try
                {
                    FamilyInstance aFamilyInst = element as FamilyInstance;
                    if (aFamilyInst.SuperComponent == null)
                    {
                        WriteFamilySub(element, writer, doc);
                    }
                }
                catch { }
                
                GeometryElement geo = element.get_Geometry(opt);
                writer.WriteStartElement("Name");
                writer.WriteString(element.Name);

                foreach (GeometryObject obj in geo)
                {
                    GeometryInstance instance = obj as GeometryInstance;
                    if (instance != null)
                    {
                        WriteFamily(instance, writer, doc);
                    }
                    else
                    {
                        Solid solid = obj as Solid;
                        if (solid == null || 0 == solid.Faces.Size || solid.Edges.Size == 0)
                        {
                            continue;
                        };
                        WriteFaceSolid(solid, writer, doc);
                    }
                }
                writer.WriteEndElement();

            }
            writer.WriteEndElement();
        }

        public void WriteFamilySub(Element anElem, XmlTextWriter writer, Document doc)
        {
            FamilyInstance aFamilyInst = anElem as FamilyInstance;
            if (aFamilyInst.SuperComponent == null)
            {
                var subElements = aFamilyInst.GetSubComponentIds();
                if (subElements.Count > 0)
                {
                    foreach (var aSubElemId in subElements)
                    {
                        var aSubElem = doc.GetElement(aSubElemId);
                        if (aSubElem is FamilyInstance)
                        {
                            WriteFamilySub(aSubElem, writer, doc);
                        }
                    }

                }
               
            }else
            {
                Options opt = new Options();
                opt.ComputeReferences = true;
                GeometryElement geo = anElem.get_Geometry(opt);
                writer.WriteStartElement("Name");
                writer.WriteString(anElem.Name);

                foreach (GeometryObject obj in geo)
                {
                    GeometryInstance instance = obj as GeometryInstance;
                    Solid solid = obj as Solid;
                    if (solid == null || 0 == solid.Faces.Size || solid.Edges.Size == 0)
                    {
                        continue;
                    };
                    WriteFaceSolid(solid, writer, doc);

                }
                writer.WriteEndElement();
            }

        }

        public void WriteFamily(GeometryInstance instance, XmlTextWriter writer, Document doc)
        {
            GeometryElement instanceGeometryElement = instance.GetSymbolGeometry();
            foreach (GeometryObject instanceObj in instanceGeometryElement)
            {
                Solid solid = instanceObj as Solid;
                if (solid == null || 0 == solid.Faces.Size || solid.Edges.Size == 0)
                {
                    GeometryInstance instanceNew = instanceObj as GeometryInstance;
                    if (instanceNew == null)
                    {
                        continue;
                    }
                    WriteFamily(instanceNew, writer, doc);
                }
                else
                {
                    WriteFaceSolidTransform(solid, writer, doc, instance);
                }
            }
        }

        public void WriteFaceSolid(Solid solid, XmlTextWriter writer, Document doc)
        {

            foreach (Face face in solid.Faces)
            {
                writer.WriteStartElement("Face");
                writer.WriteStartElement("Material");
                try
                {
                    string nameMaterial = doc.GetElement(face.MaterialElementId).Name;
                    writer.WriteString(nameMaterial);
                    writer.WriteEndElement();
                }
                catch
                {
                    writer.WriteString("NoMaterial");
                    writer.WriteEndElement();
                }

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

        public void WriteFaceSolidTransform(Solid solid, XmlTextWriter writer, Document doc, GeometryInstance instance)
        {
            Transform instTransform = instance.Transform;
            foreach (Face face in solid.Faces)
            {
                writer.WriteStartElement("Face");

                writer.WriteStartElement("Material");
                try
                {
                    string nameMaterial = doc.GetElement(face.MaterialElementId).Name;
                    writer.WriteString(nameMaterial);
                    writer.WriteEndElement();
                }
                catch
                {
                    writer.WriteString("NoMaterial");
                    writer.WriteEndElement();
                }
                Mesh mesh = face.Triangulate();
                for (int i = 0; i < mesh.NumTriangles; i++)
                {
                    MeshTriangle triangle = mesh.get_Triangle(i);
                    for (int g = 0; g < 3; g++)
                    {
                        writer.WriteStartElement("Point");
                        XYZ point = triangle.get_Vertex(g);
                        XYZ transformedPoint = instTransform.OfPoint(point);
                        writer.WriteString(transformedPoint.ToString());
                        writer.WriteEndElement();
                    }
                };
                writer.WriteEndElement();
            }
        }
    }
}
