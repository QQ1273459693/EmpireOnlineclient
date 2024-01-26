#if UNITY_EDITOR
#endif
using UnityEngine;
using Water2D;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public abstract class WaterFeatureLayerRenderer : MonoBehaviour
{
    [HideInInspector] [SerializeField] protected LayerRenderer _layerRenderer;
    [HideInInspector] [SerializeField] bool _run;
    [HideInInspector] [SerializeField] public bool run
    {
        get { return _run; }
        set
        {
            _layerRenderer.run = value;
            if (value) runTrue++;
            if (!value) runFalse++;
            _run = value;
        }
    }
    private int runTrue = 0;
    private int runFalse = 0;

    private void RunThisFrame()
    {
        int rt = runTrue;
        int rf = runFalse;
        runTrue = runFalse = 0;
        if (rt > 0) _run = true;
        else if (rt == 0 && rf > 0) _run = false;
    }

    protected virtual void Update()
    {
        RunThisFrame();
        _layerRenderer.run = run;
    }
}
