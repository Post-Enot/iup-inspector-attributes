using IUP.Toolkits.InspectorAttributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public sealed class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement propertyField = new PropertyField(property);
        propertyField.SetEnabled(false);
        return propertyField;
    }
}
