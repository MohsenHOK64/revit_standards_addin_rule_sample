using Autodesk.Revit.DB;
using System.Linq;
using System.Collections.Generic;

public class ListViewsInSelectedPrintSet
{
    public IEnumerable<ElementId> Run(Document doc, List<ElementId> ids)
    {
        PrintManager pm = doc.PrintManager;

        // Ensure that the print manager setting is set to selected views
        if (pm.PrintRange != PrintRange.Select)
        {
            pm.PrintRange = PrintRange.Select;
        }

        ViewSheetSetting existingVSS = pm.ViewSheetSetting;

        ViewSheetSet existingVSSet = (ViewSheetSet)existingVSS.CurrentViewSheetSet;

        // Get all of the sheets in the document
        List<ElementId> sheetsInDocIds = new FilteredElementCollector(doc)
            .OfClass(typeof(ViewSheet))
            .ToElements()
            .Cast<ViewSheet>()
            .Select(s => s.Id)
            .ToList();

        List<ElementId> sheetsInModel = new List<ElementId>();

        foreach (ElementId viewId in sheetsInDocIds)
        {
            View view = (View)doc.GetElement(viewId);
            sheetsInModel.Add(viewId);
        }

        // Add the excluded sheets to the sheet set
        ViewSet newVSS = new ViewSet();
        foreach (var viewId in sheetsInModel)
        {
            View currView = (View)doc.GetElement(viewId);
            if (!newVSS.Contains(currView))
            {
                newVSS.Insert(currView);
            }
            else
            {
                continue;
            }
        }
        using (Transaction tr = new Transaction(doc, "Add Sheets to View Sheet Set"))
        {
            if (doc.IsModifiable)
            {
                try
                {
                    existingVSS.CurrentViewSheetSet.Views = newVSS;
                    existingVSS.Save();
                }
                catch
                {
                    _ = ex.Message;
                    // View Sheet Set is unchanged, therefore do nothing and just commit it as is
                }
            }
            else
            {
                tr.Start();
                try
                {
                    existingVSS.CurrentViewSheetSet.Views = newVSS;
                    existingVSS.Save();
                }
                catch
                {
                    _ = ex.Message;
                    // View Sheet Set is unchanged, therefore do nothing and just commit it as is
                }
                tr.Commit();
            }
        }
        return new List<ElementId>();
    }
}
