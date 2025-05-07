using Autodesk.Revit.DB;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.UI;

public class ListViewsInSelectedPrintSet
{
    public IEnumerable<ElementId> Run(Document doc, List<ElementId> ids)
    {
        // First ensure that the "Add ALL Sheets to HOK Print Set" parameter exists and is enabled
        var hokPrintSetParamId = GlobalParametersManager.FindByName(
            doc,
            "Add ALL Sheets to HOK Print Set"
        );

        // If parameter is not found, create it
        if (hokPrintSetParamId == ElementId.InvalidElementId)
        {
            // Creating the parameter
            using (Transaction tr = new Transaction(doc, "Create HOK Print Set Parameter"))
            {
                if (doc.IsModifiable)
                {
                    GlobalParameter hokPrintSetParam = GlobalParameter.Create(
                        doc,
                        "Add ALL Sheets to HOK Print Set",
                        SpecTypeId.Boolean.YesNo
                    );
                    // Set the default value to off/false
                    hokPrintSetParam.SetValue(new IntegerParameterValue(1));
                }
                else
                {
                    tr.Start();
                    GlobalParameter hokPrintSetParam = GlobalParameter.Create(
                        doc,
                        "Add ALL Sheets to HOK Print Set",
                        SpecTypeId.Boolean.YesNo
                    );
                    // Set the default value to off/false
                    hokPrintSetParam.SetValue(new IntegerParameterValue(1));
                    tr.Commit();
                }
                var hokParameter = GlobalParametersManager.FindByName(
                    doc,
                    "Add ALL Sheets to HOK Print Set"
                );
                // Stop executing after creating it
                TaskDialog.Show(
                    "Created New Global Parameter",
                    "Successfully created the "
                        + hokParameter.Name
                        + " parameter and set its value to "
                        + (hokParameter.GetValue() as IntegerParameterValue).Value
                );
            }
        }
        hokPrintSetParamId = GlobalParametersManager.FindByName(
            doc,
            "Add ALL Sheets to HOK Print Set"
        );
        var hokPrintSetParameter = doc.GetElement(hokPrintSetParamId) as GlobalParameter;
        if ((hokPrintSetParameter.GetValue() as IntegerParameterValue).Value == 1)
        {
            goto AddAllSheets;
        }
        else
        {
            return new List<ElementId>();
        }

        AddAllSheets:
        PrintManager pm = doc.PrintManager;

        // Ensure that the print manager setting is set to selected views
        if (pm.PrintRange != PrintRange.Select)
        {
            pm.PrintRange = PrintRange.Select;
        }

        ViewSheetSetting existingVSS = pm.ViewSheetSetting;
        // Save the previously selected viewsheet set so we can set it back to the original at the end
        var existingViewSheetSet = existingVSS.CurrentViewSheetSet;

        var hokSheetSet = new FilteredElementCollector(doc)
            .OfClass(typeof(ViewSheetSet))
            .Cast<ViewSheetSet>();

        if (hokSheetSet.Select(ss => ss.Name).Contains("_HOK Automated Print Set"))
            existingVSS.CurrentViewSheetSet = hokSheetSet.First(
                ss => ss.Name == "_HOK Automated Print Set"
            );

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
                if (
                    hokSheetSet != null
                    && ((ViewSheetSet)existingVSS.CurrentViewSheetSet).Name
                        == "_HOK Automated Print Set"
                )
                {
                    try
                    {
                        existingVSS.CurrentViewSheetSet.Views = newVSS;
                        existingVSS.Save();
                    }
                    catch { }
                }
                else
                {
                    existingVSS.CurrentViewSheetSet.Views = newVSS;
                    existingVSS.SaveAs("_HOK Automated Print Set");
                }
                // existingVSS.CurrentViewSheetSet = existingViewSheetSet;
                // existingVSS.Save();
            }
            else
            {
                tr.Start();
                if (
                    hokSheetSet != null
                    && ((ViewSheetSet)existingVSS.CurrentViewSheetSet).Name
                        == "_HOK Automated Print Set"
                )
                {
                    try
                    {
                        existingVSS.CurrentViewSheetSet.Views = newVSS;
                        existingVSS.Save();
                    }
                    catch { }
                }
                else
                {
                    existingVSS.CurrentViewSheetSet.Views = newVSS;
                    existingVSS.SaveAs("_HOK Automated Print Set");
                }
                // existingVSS.CurrentViewSheetSet = existingViewSheetSet;
                // existingVSS.Save();
                tr.Commit();
            }
        }
        return new List<ElementId>();
    }
}
