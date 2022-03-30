using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining33
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Веберите трубу");
            var selectedElement = doc.GetElement(selectedRef);

            if (selectedElement is Pipe)
            {
                Parameter lengthParameter = selectedElement.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                if (lengthParameter.StorageType == StorageType.Double)
                {
                    double lengthValue = UnitUtils.ConvertFromInternalUnits(lengthParameter.AsDouble(), UnitTypeId.Meters);
                    double marginLength = lengthValue * 1.1;
                    var categorySet = new CategorySet();
                    categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));

                    using (Transaction ts = new Transaction(doc, "Устанавливаем параметр"))
                    {
                        ts.Start();
                        var pipe = selectedElement as Pipe;
                        Parameter lParameter = pipe.LookupParameter("Комментарии");
                        lParameter.Set(marginLength.ToString());
                        
                        ts.Commit();
                    }


                    TaskDialog.Show("Длина трубы с запасом", marginLength.ToString());
                   
                }
                
            }
            return Result.Succeeded;
        }

    }

}
