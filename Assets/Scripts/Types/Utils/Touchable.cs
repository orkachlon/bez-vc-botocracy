// file Touchable.cs
// Correctly backfills the missing Touchable concept in Unity.UI's OO chain.

using UnityEditor;
using UnityEngine.UI;

namespace Types.Utils {
#if UNITY_EDITOR
    [CustomEditor(typeof(Touchable))]
    public class TouchableEditor : Editor
    { public override void OnInspectorGUI() { } }
#endif
    
    public class Touchable : Graphic {
        protected override void UpdateGeometry() { }
    }
}