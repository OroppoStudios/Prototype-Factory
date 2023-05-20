using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class CustomEditor : EditorWindow
{

    [SerializeField]
    private VisualTreeAsset m_UXMLTree;

    [MenuItem("Window/UI Toolkit/CustomEditor")]
    public static void ShowExample()
    {
        CustomEditor wnd = GetWindow<CustomEditor>();
        wnd.titleContent = new GUIContent("CustomEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/CustomEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        root.Add(m_UXMLTree.Instantiate());
    }
}