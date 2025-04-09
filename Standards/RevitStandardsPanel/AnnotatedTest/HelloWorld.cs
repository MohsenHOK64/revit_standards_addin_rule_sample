using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

// change the name of the class to match the name of the cs file
public class HelloWorld
{
	// doc = active document
	// ids = the added or changed elements, or <null> if running the rule on the entire document
	public IEnumerable<ElementId> Run(Document doc, List<ElementId> ids)
	{
		TaskDialog hwDialog = new TaskDialog("Custom Rule");
		hwDialog.MainInstruction = "Hello World!";
		hwDialog.MainContent = "This sample shows how to call a Revit task dialog from a custom rule.";
		hwDialog.Show();
		
		// return the ids of elements that failed the rule, or <null>
		return null;
	}
}
