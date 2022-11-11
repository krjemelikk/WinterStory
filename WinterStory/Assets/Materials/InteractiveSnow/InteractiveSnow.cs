using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSnow : MonoBehaviour
{
    [SerializeField] private CustomRenderTexture _snowHeightMap;
    [SerializeField] private Material _heightMapUpdate;
    [SerializeField] private Transform[] _steps;
    private Camera _mainCamera;
    private int _index;

    private static readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
    private static readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
    
    private void Start()
    {
        _heightMapUpdate.SetVector(DrawPosition, new Vector4(-1, -1, 0, 0));
        _snowHeightMap.Initialize();
        //_mainCamera = Camera.main;
    }

    private void Update()
    {
        DrawWithSteps();
        _snowHeightMap.Update();
    }

    private void DrawWithSteps()
    {
        var step = _steps[_index++ % _steps.Length];

        Ray ray = new Ray(step.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
        {
            if (hit.transform.tag == "InteractiveSnow")
            {
                Vector2 hitTextureCoord = hit.textureCoord;
                float angle = step.transform.rotation.eulerAngles.y;

                _heightMapUpdate.SetVector(DrawPosition, hitTextureCoord);
                _heightMapUpdate.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
            }
        }
    }

    private void DrawByMouse()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var coord = hit.textureCoord;
                _heightMapUpdate.SetVector(DrawPosition, coord);
            }
        }
    }
}
