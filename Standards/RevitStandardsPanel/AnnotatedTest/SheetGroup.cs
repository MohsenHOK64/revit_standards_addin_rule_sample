using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class SheetGroup
{
    public IEnumerable<ElementId> Run(Document doc, List<ElementId> ids)
    {
        List<ViewSheet> sheets = null;
        if (ids == null)
        {
            sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .ToList();
        }
        else
        {
            sheets = ids.Select(q => doc.GetElement(q)).Where(q => q is ViewSheet).Cast<ViewSheet>().ToList();
        }

        // set a 'Sheet Group' parameter to be the first two characters
        foreach (var sheet in sheets)
        {
            var groupParam = sheet.LookupParameter("Sheet Group");
            if (groupParam == null)
            {
                return null;
            }
            var number = sheet.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
            var length = 2;
            if (number.Length < 2)
            {
                length = number.Length;
            }
            groupParam.Set(number.Substring(0, length));
        }
        return null;
    }       
}
