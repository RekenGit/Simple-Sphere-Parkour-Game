using UnityEngine;

public class UIPlayerCustomization : MonoBehaviour
{
    [SerializeField] private MeshFilter playerMesh;
    [SerializeField] private MeshFilter playerPrefabMesh;
    [SerializeField] private Material meshMaterial;
    [SerializeField] private Material UIMaterial;

    private PlayerCustomizationObject customizations;

    private void Start()
    {
        meshMaterial.SetFloat("_TreshHold", 0.25f);
        customizations = GameManager.Instance.GetPlayerCustomization();
        SetPlayerCustomization(false);
    }

    public void ChangeBaseColor(bool isGoingRight)
    {
        customizations.selectedBaseColorId = (customizations.selectedBaseColorId + (isGoingRight ? 1 : -1 + customizations.baseColors.Length)) % customizations.baseColors.Length;
        SetPlayerCustomization();
    }

    public void ChangeGlowColor(bool isGoingRight)
    {
        customizations.selectedGlowColorId = (customizations.selectedGlowColorId + (isGoingRight ? 1 : -1 + customizations.glowColors.Length)) % customizations.glowColors.Length;
        SetPlayerCustomization();
    }

    public void ChangeMesh(bool isGoingRight)
    {
        customizations.selectedMeshId = (customizations.selectedMeshId + (isGoingRight ? 1 : -1 + customizations.sphereMeshes.Length)) % customizations.sphereMeshes.Length;
        SetPlayerCustomization();
    }

    private void SetPlayerCustomization(bool setPrefs = true)
    {
        int _base = customizations.selectedBaseColorId;
        int _glow = customizations.selectedGlowColorId;
        int _mesh = customizations.selectedMeshId;

        meshMaterial.SetColor("_Color", customizations.baseColors[_base]);
        meshMaterial.SetColor("_EmissionColor", customizations.glowColors[_glow]);
        UIMaterial.SetColor("_EmissionColor", customizations.glowColors[_glow]);
        playerMesh.mesh = customizations.sphereMeshes[_mesh];
        playerPrefabMesh.mesh = customizations.sphereMeshes[_mesh];

        if (!setPrefs) return;
        PlayerPrefs.SetInt("PlayerBaseColorId", _base);
        PlayerPrefs.SetInt("PlayerGlowColorId", _glow);
        PlayerPrefs.SetInt("PlayerMeshId", _mesh);
    }
}
