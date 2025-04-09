# Rule for Checking Views in Current Print Set

```json
{
  "Workset Rules":
  [],
  "Parameter Rules": 
  [
    {
      "Rule Name": "Check Views in Print Set",
      "Element Classes": ["Autodesk.Revit.DB.ViewSheet"],
      "Custom Code": "ListViewsInSelectedPrintSet",
      "User Message": "Added all sheets to current print set",
      "When Run": ["SyncToCentral"]
    }
  ]
}
```
