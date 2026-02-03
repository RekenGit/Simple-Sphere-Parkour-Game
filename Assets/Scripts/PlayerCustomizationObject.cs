using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCustomizationObject", menuName = "Scriptable Objects/PlayerCustomizationObject")]
public class PlayerCustomizationObject : ScriptableObject
{
    public Color[] baseColors;
    [ColorUsage(showAlpha: true, hdr: true)]
    public Color[] glowColors;
    public Mesh[] sphereMeshes;

    public int selectedBaseColorId = 0;
    public int selectedGlowColorId = 0;
    public int selectedMeshId = 0;
}
