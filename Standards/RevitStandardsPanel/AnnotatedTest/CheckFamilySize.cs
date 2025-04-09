using System.Collections.Generic;
using System.Linq;
using System.IO;
using Autodesk.Revit.DB;

public class CheckFamilySize
{
	public IEnumerable<ElementId> Run(Document doc, List<ElementId> ids)
	{
		const int maxKBsize = 100;
			  
		List<Family> families = null;
        if (ids == null)
        {
            families = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(q => q.IsEditable)
                .ToList();
        }
        else
        {
            families = ids.Select(q => doc.GetElement(q)).Where(q => q is Family).Cast<Family>().ToList();
        }

        var ret = new List<ElementId>();
        foreach (var family in families)
        {
            // Can't call EditFamily from Dynamic Update, so need to look at some other proxy for model size other than size on disk
            
            // var famDoc = doc.EditFamily(family);
            // var tempFile = Path.GetTempFileName() + ".rfa";
            // famDoc.SaveAs(tempFile);
            // var bytes = new System.IO.FileInfo(tempFile).Length;
            // var kb = ConvertBytesToKb(bytes);
            // if (kb > maxKBsize)
            // {
            //     ret.Add(family.Id);
            // }
        }
        return ret;
	}   
}
