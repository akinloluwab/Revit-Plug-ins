using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Window
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and documnet objects
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uiapp.ActiveUIDocument.Document;
            //Document doc = commandData.Application.ActiveUIDocument.Document;
            List <ElementId> ids = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType().ToElementIds().ToList(); //list of windows
            FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsElementType().OfCategory(BuiltInCategory.OST_GenericModel); //collector for tags
            //collector = collector.OfClass(typeof(FamilyInstance));
            //collector = collector.OfCategory(BuiltInCategory.OST_GenericModel); //tags are generic models
            //var query = from element in collector where element.Name.Contains("BB_Hazard_Marker_2017") select element; //name of tags

            List<Element> hazardTag = collector.Where(c => c.Name.Contains("BB_Hazard_Marker_2017")).ToList();
            ElementId symbolId = hazardTag.FirstOrDefault().Id;
            try
            {
                foreach (ElementId elId in ids)
                {
                    //Element element = doc.GetElement(symbolId); //gets tag
                    Element elementWin =  doc.GetElement(elId);  //gets windows                  
                    LocationPoint LP =  elementWin.Location as LocationPoint; 
                    XYZ ElementPoint = LP.Point as XYZ; //XYZ position of windows
                    //string x = LP.Point.X.ToString();
                    //string y = LP.Point.Y.ToString();
                    //string z = LP.Point.Z.ToString();

                    FamilySymbol element = doc.GetElement(symbolId) as FamilySymbol; 
                    Transaction trans = new Transaction(doc);
                    trans.Start("Lab");
                    element.Activate();
                    doc.Create.NewFamilyInstance(ElementPoint, element, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    //doc.FamilyCreate.PlaceGroup(ElementPoint, group.GroupType);
                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Copy Paste", "Error: " + e);
            }
               
                
            return Result.Succeeded;
        }


    }

}
/*
            
            //Define a reference Object to accept the pick result
            Reference pickedref = null;

            //Pick a group
            Selection sel = uiapp.ActiveUIDocument.Selection;
            pickedref = sel.PickObject(ObjectType.Element, "Please select a group");
            Element elem = doc.GetElement(pickedref);
            //Group group = elem as Group;

            //Pick point
            XYZ point = sel.PickPoint("Please pick a point to place group");

            //Place the group
            //Transaction trans = new Transaction(doc);
            //trans.Start("Lab");
            //doc.Create.PlaceGroup(point, group.GroupType);
           // trans.Commit();*/
