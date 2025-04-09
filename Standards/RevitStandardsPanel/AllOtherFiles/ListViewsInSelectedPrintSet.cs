using Autodesk.Revit.DB;
using System.Linq;
using System.Collections.Generic;

public class ListViewsInSelectedPrintSets
{
    public IEnumerable<ElementId> Run(Document doc, List<ElementId> ids)
    {
        PrintManager pm = doc.PrintManager;

        ViewSheetSetting existingVSS = pm.ViewSheetSetting;

        ViewSheetSet existingVSSet = (ViewSheetSet)existingVSS.CurrentViewSheetSet;

        // Add existing sheets not in the ViewSheetSet to it

        // Get all of the sheets in the document
        List<ElementId> sheetsInDocIds = new FilteredElementCollector(doc)
            .OfClass(typeof(ViewSheet))
            .ToElements()
            .Cast<ViewSheet>()
            .Select(s => s.Id)
            .ToList();

        List<ElementId> sheetsNotInSheetSet = new List<ElementId>();

        foreach (ElementId viewId in sheetsInDocIds)
        {
            View view = (View)doc.GetElement(viewId);
            if (!existingVSSet.Views.Contains(view))
            {
                sheetsNotInSheetSet.Add(viewId);
            }
        }

        // Add the excluded sheets to the sheet set
        ViewSet newVSS = new ViewSet();
        foreach (var viewId in sheetsNotInSheetSet)
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
            tr.Start();
            existingVSS.CurrentViewSheetSet.Views = newVSS;
            existingVSS.Save();
            tr.Commit();
        }
        return sheetsNotInSheetSet;
    }
}
