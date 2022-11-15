using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractiveSnow : MonoBehaviour
{
    [SerializeField] private Transform[] _steps;
    
    private Material _heightMapUpdate;
    private CustomRenderTexture _snowHeightMap;
    
    private int _index;
    private float timeToRestoreOneTick;
    private float SecondsToRestore = 50;

    private static readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
    private static readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
    private static readonly int DrawBrush = Shader.PropertyToID("_DrawBrush");
    private static readonly int HeightMap = Shader.PropertyToID("_HeightMap");
    private static readonly int RestoreAmount = Shader.PropertyToID("_RestoreAmount");

    private CustomRenderTexture CreateHeightMap(int weight, int height, Material material)
    {
        var texture = new CustomRenderTexture(weight, height);
       
        texture.dimension = TextureDimension.Tex2D;
        texture.format = RenderTextureFormat.R8;
        texture.material = material;
        texture.updateMode = CustomRenderTextureUpdateMode.Realtime;
        texture.doubleBuffered = true;

        return texture;
    }

    private Material CreateHeightMapUpdate(Shader shader, Texture stepPrint)
    {
        var material = new Material(shader);
        material.SetTexture(DrawBrush, stepPrint);
        material.SetVector(DrawPosition, new Vector4(-1, -1, 0, 0));
        return material;
    }

    private Material CreateMaterial(Shader shader, Texture stepPrint)
    {
        var material = new Material(shader);
        material.SetTexture(DrawBrush, stepPrint);
        material.SetVector(DrawPosition, new Vector4(-1, -1, 0, 0));
        return material;
    }

    private void Start()
    {
        var shader = Resources.Load<Shader>(@"Terrain/InteractiveSnow/Shader/SnowHeightMapUpdate");
        var stepPrint = Resources.Load<Texture>(@"Terrain/InteractiveSnow/Textures/CircleStepPrint");
        var materialParent = Resources.Load<Material>(@"Terrain/InteractiveSnow/Material/Snow");

        var material = new Material(materialParent);

        _heightMapUpdate = CreateHeightMapUpdate(shader, stepPrint);
        _snowHeightMap = CreateHeightMap(512,512, _heightMapUpdate);

        var terrain = gameObject.GetComponent<Terrain>();
        terrain.materialTemplate = material;
        terrain.materialTemplate.SetTexture(HeightMap, _snowHeightMap);

        _snowHeightMap.Initialize();
    }

    private void Update()
    {
        DrawWithSteps();
        timeToRestoreOneTick -= Time.deltaTime;
        if (timeToRestoreOneTick < 0)
        {
            // Если в этот update мы хотим увеличить цвет всех пикселей карты высот на 1
            _heightMapUpdate.SetFloat(RestoreAmount, 1 / 250f);
            timeToRestoreOneTick = SecondsToRestore / 250f;
        }
        else
        {
            // Если не хотим
            _heightMapUpdate.SetFloat(RestoreAmount, 0);
        }
        _snowHeightMap.Update();
    }

    private void DrawWithSteps()
    {
        var step = _steps[_index++ % _steps.Length];

        Ray ray = new Ray(step.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
        {
            if (hit.collider.name == gameObject.name)
            {
                Vector2 hitTextureCoord = hit.textureCoord;
                float angle = step.transform.rotation.eulerAngles.y;

                _heightMapUpdate.SetVector(DrawPosition, hitTextureCoord);
                _heightMapUpdate.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
            }
        }
    }
}
