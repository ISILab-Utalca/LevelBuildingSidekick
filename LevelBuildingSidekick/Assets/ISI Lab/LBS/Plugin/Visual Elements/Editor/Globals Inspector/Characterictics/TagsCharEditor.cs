using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor(typeof(LBSTagsCharacteristic))]
public class TagsCharEditor : LBSCustomEditor
{
    public TextField idField;
    public DropdownField dropdownField;

    public TagsCharEditor()
    {
        idField = new TextField();
        this.Add(idField);

        dropdownField = new DropdownField();
        this.Add(dropdownField);
    }

    public override void SetInfo(object target)
    {
        var t = target as LBSTagsCharacteristic;

        idField.value = t.ToString();
    }
}
