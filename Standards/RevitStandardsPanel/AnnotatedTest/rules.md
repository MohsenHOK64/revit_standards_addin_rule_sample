# Sample Annotated Rules File

This annotated Rules File is provided to both demonstrate how each rule type is configured _and_ to illustrate how a Rule File can be annotated. All [Github Flavored Markdown](https://github.github.com/gfm/) should be allowed, intersperced between closed JSON code blocks. Multiple code blocks for each of the two types of rules are allowed as long as they are identified as either a "Workset Rules" or a "Parameter Rules" block. Multiple rules are allowed within a block as well - there are some examples of this below.

![image](https://github.com/user-attachments/assets/16baf1f7-6107-4184-b527-122782e21e17)

Additional documentation can be found in the Readme of the [revit_standards_addin](https://github.com/InnovationDesignConsortium/revit_standards_addin) repository.

## Workset Rules

### Levels and Grids
This rule will enforce keeping Levels and Grids on the default Shared Levels and Grids workset. Notice that when no parameters are specified, all elements of the specified categories are captured by this rule.

```json
{
  "Workset Rules":
  [
    {
      "Categories": ["Levels", "Grids"],
      "Workset": "Shared Levels and Grids",
      "Parameters": []
    }
  ]
}
```

### Level 1 Stuff (Furniture and Entourage)
Elements from the _Revit Categories_:
- Furniture
- Entourage

Will be moved to the _Workset_:
- Level 1 Stuff

When they have a _Level_ value of:
- Level 1

```json
{
  "Workset Rules":
  [
    {
      "Categories": ["Furniture", "Entourage"],
      "Workset": "Level 1 Stuff",
      "Parameters":
      [
        {"Name": "Level", "Value": "Level 1"}
      ],
      "When Run": ["Save", "SyncToCentral"]
    }
  ]
}
```

### Level 1 Stuff (Walls)
Elements from the _Revit Category_:
- Walls

Will be moved to the _Workset_:
- Level 1 Stuff

When the elements meet multiple requirements. Note that Revit Walls do not have a _Level_ property, so for this rule, we must specify the _Base Constraint_ is:
- Level 1

And the _Function_ is:
- Interior

```json
{
  "Workset Rules":
  [
    {
      "Categories": ["Walls"],
      "Workset": "Level 1 Stuff",
      "Parameters":
      [
        {"Name": "Base Constraint", "Value": "Level 1"},
        {"Name": "Function", "Value": "Interior"}
      ]
    }
  ]
}
```

### Level 2 Stuff (Furniture and Entourage)
Elements from the _Revit Categories_:
- Furniture
- Entourage

Will be moved to the _Workset_:
- Level 2 Stuff

When they have a _Level_ value of:
- Level 2

```json
{
  "Workset Rules":
  [
    {
      "Categories": ["Furniture", "Entourage"],
      "Workset": "Level 2 Stuff",
      "Parameters":
      [
        {"Name": "Level", "Value": "Level 2"},
        {"Name": "Auto Assign Workset", "Value": "1"}
      ]
    }
  ]
}
```

## Parameter Rules

### List Rules
- The first list rule limits the _Comments_ parameter on all elements in the **Walls** category to values of either 1, 2, or 3. The values are enumerated in the rule.
- The second list rule ... The values are enumerated in an external CSV file.

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Comments Rule For Walls",
      "Categories": ["Walls"],
      "Parameter Name": "Comments",
      "User Message": "Comments must be 1, 2, or 3",
      "List Options":
      [
        {"name": "1", "description": ""},
        {"name": "2", "description": ""},
        {"name": "3", "description": ""}
      ]
    },
    {
      "Rule Name": "Door Comments from CSV",
      "Categories": ["Doors"],
      "Parameter Name": "Comments",
      "Is Value Required": false,
      "User Message": "Comments must be a, b, or c",
      "List Source": "DoorCommentsValues.csv"
    }
  ]
}
```

### Key Value Rules
- The first rule sets some values for Rooms based on the key values defined in the rule configuration below.
- The second rule uses an external CSV file to provide the list of parameters and values. Note that this relies on a key value being stored in a Global Parameter

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Room Finish Keys",
      "Categories": ["Rooms"],
      "Parameter Name": "Room Style",
      "Driven Parameters": ["Wall Finish", "Floor Finish", "Ceiling Finish"],
      "Key Values": [
        ["A", "Wall A", "Floor A", "Ceiling A"],
        ["B", "Wall B", "Floor B", "Ceiling B"],
        ["C", "Wall C", "Floor C", "Ceiling C"]
      ]
    },
    {
      "Rule Name": "Code Occupancy",
      "Categories": ["Rooms"],
      "Key Value Path": "BuildingCodeOccupancy.csv"
    }
  ]
}
```

### Format Rule - Wall Type Name

The example here requires each Wall Type Name to be constructed out of the values of three of its type parameters. Note that this format requirement is a prefix, so additional values after the format pattern will not invalidate the Type Name.

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Set Wall Type Function",
      "Element Classes": ["Autodesk.Revit.DB.WallType"],
      "Parameter Name": "Type Name",
      "Format": "{Function} - {Structural Material} - {Width}",
      "User Message": "Type name does not match required format"
    }
  ]
}
```

### Requirement Rules

Below are three different requirement rules. These require certain parameters to have a specific, mathematical relationship to other parameters of the same element.

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Roof Offset",
      "Categories": ["Roofs"],
      "Parameter Name": "Base Offset From Level",
      "Requirement": "IF {Cutoff Offset} > 0 THEN {Base Offset From Level} > 0",
      "User Message": "IF {Cutoff Offset} > 0 THEN {Base Offset From Level} > 0"
    },
    {
      "Rule Name": "Window Sill Height",
      "Categories": ["Windows"],
      "Parameter Name": "Sill Height",
      "Requirement": "> {Width}",
      "User Message": "Sill height must greater than width"
    },
    {
      "Rule Name": "Door Sill Height",
      "Categories": ["Doors"],
      "Parameter Name": "Sill Height",
      "Requirement": "= 0",
      "User Message": "Sill height must be 0"
    }
  ]
}
```

### Regex Rule - Mark is a Number

This is a simple example of defining a regular expression that the values of a specified parameter must meet - in this case, the Mark must be a number. You can imagine defining a specific pattern for a Project Number, zip code, or phone number as well.

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Mark is Number",
      "User Message": "Mark must be a number",
      "Categories": ["<all>"],
      "Parameter Name": "Mark",
      "Regex": "^[0-9]+$"
    }
  ]
}
```

### Formula Rule - Number of Occupants in a Room

Mathematical operations can be performed on parameters within the same element. In this example, the built-in _Area_ value is divided by the _Occupant Load Factor_ (maximum floor area allowance per occupant, Data Type: Area) and the result will be written to the _Occupancy Load_ parameter (Data Type: Integer).

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Room Occupancy Load",
      "Categories": ["Rooms"],
      "Parameter Name": "Occupancy Load",
      "Formula": "{Area} / {Occupant Load Factor}"
    }
  ]
}
```

### From Host Instance Rule

This rule sets the value of a parameter in a hosted element equal to the value of another parameter in the host element. In this case, the parameters are the orientation of doors and windows.

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Insert Orientation = Host Insert",
      "Categories": ["Doors", "Windows"],
      "Parameter Name": "Orientation",
      "From Host Instance": "Orientation",
      "User Message": "The Orientation of an insert must equal the Orientation of its host"
    }
  ]
}
```

### Prevent Duplicates Rule

This rule prevents duplicate values in a parameter for elements of the same, specified category.

```json
{
  "Parameter Rules": 
  [
    {
      "Rule Name": "Room Number Dup",
      "Categories": ["Rooms"],
      "User Message": "Room Number cannot duplicate an existing value",
      "Parameter Name": "Number",
      "Prevent Duplicates": "True"
    }
  ]
}
```

### Custom Code Rules
- The first custom code rule simply opens a task dialog on opening the file and when clicking the "Run" button in the Properties Panel. 
- The second custom code rule limits the number of in-place families allowed in the project. The limit and the logic are defined in the referenced CS file.
- The third custom code rule sets the value of a parameter on any family according to where it is in plan. The parameter and the logic are defined in the referenced CS file.
- The fourth custom code rule sets the value of the Sheet Group parameter on a sheet to the first two characters of the Sheet Number.

```json
{
  "Parameter Rules": 
  [
   {
      "Rule Name": "Hello World",
      "Element Classes": ["Project Info"],
      "Custom Code": "HelloWorld",
      "User Message": "Hello World!",
      "When Run": ["Open"]
    },
    {
      "Rule Name": "In Place Family Quantity Limit",
      "Element Classes": ["Autodesk.Revit.DB.FamilyInstance"],
      "Custom Code": "InPlaceFamilyCheck",
      "User Message": "There are too many In-Place Families in the model."
    },
    {
      "Rule Name": "Set Quadrant",
      "Element Classes": ["Autodesk.Revit.DB.FamilyInstance"],
      "Custom Code": "SetQuadrant"
    },
    {
      "Rule Name": "Sheet Group",
      "Element Classes": ["Autodesk.Revit.DB.ViewSheet"],
      "Custom Code": "SheetGroup",
      "User Message": "Sheet Group parameter set to first two characters of Sheet Number"
    }
  ]
}
```
