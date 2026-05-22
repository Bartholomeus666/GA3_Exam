using UnityEngine;

/// <summary>
/// Attach this to your main Directional Light (the sun).
/// Each frame it pushes the light's direction and color into
/// global shader properties that any shader can read.
/// </summary>
[ExecuteAlways]
public class MainLightShaderData : MonoBehaviour
{
    // These names must match the globals declared in the .hlsl file.
    private static readonly int DirID   = Shader.PropertyToID("_MainLightDirection");
    private static readonly int ColorID = Shader.PropertyToID("_MainLightColor");

    private Light _light;

    void OnEnable()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        if (_light == null) return;

        // Direction FROM the light (same convention as URP's mainLight.direction)
        Vector4 dir = -transform.forward;
        Shader.SetGlobalVector(DirID, dir);

        // Linear-space color × intensity
        Color c = _light.color * _light.intensity;
        Shader.SetGlobalVector(ColorID, new Vector4(c.r, c.g, c.b, 1f));
    }
}
