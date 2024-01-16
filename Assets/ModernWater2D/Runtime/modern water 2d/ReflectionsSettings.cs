using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Water2D
{
    [Serializable]
    public class ReflectionsSettings
    {
        public WaterCryo<bool> enableTopDownReflections = new WaterCryo<bool>(true);
        public WaterCryo<bool> enablePlatformerReflections = new WaterCryo<bool>(false);

        public Camera mainCamera;
        public WaterCryo<float> textureResolution;
        public WaterCryo<bool> overrideMainCamera;
        public WaterCryo<bool> reflectionObjectsVisible;
        public WaterCryo<bool> cameraVisible;
        public WaterCryo<bool> defaultReflectionSprflipx;
        public WaterCryo<float> angle;
        public WaterCryo<float> tilt;
        public WaterCryo<float> length;
        public WaterCryo<float> originalColor;
        public WaterCryo<Color> color;
        public WaterCryo<float> alpha;

        public WaterCryo<float> mirrorY;
        public List<int> layers;

        public WaterCryo<bool> usePerspective;
        public WaterCryo<Vector2> waterPerspective;
        public WaterCryo<Vector2> reflectionsPerspective;

        public WaterCryo<float> falloffStrength;
        public WaterCryo<float> falloffStart;
        public WaterCryo<bool> enableFalloff;
        public WaterCryo<bool> enableScrolling;
        public Transform playerPosition;
        public WaterCryo<float> scrollingStrength;

        public WaterCryo<bool> DistortionFPRH;

        internal void onValueChanged(UnityAction onReflectionsChanged)
        {

            textureResolution.onValueChanged = onReflectionsChanged;
            overrideMainCamera.onValueChanged = onReflectionsChanged;
            reflectionObjectsVisible.onValueChanged = onReflectionsChanged;
            cameraVisible.onValueChanged = onReflectionsChanged;
            defaultReflectionSprflipx.onValueChanged = onReflectionsChanged;
            angle.onValueChanged = onReflectionsChanged;
            tilt.onValueChanged = onReflectionsChanged;
            length.onValueChanged = onReflectionsChanged;
            originalColor.onValueChanged = onReflectionsChanged;
            color.onValueChanged = onReflectionsChanged;
            alpha.onValueChanged = onReflectionsChanged;
            mirrorY.onValueChanged = onReflectionsChanged;
            usePerspective.onValueChanged = onReflectionsChanged;
            waterPerspective.onValueChanged = onReflectionsChanged;
            reflectionsPerspective.onValueChanged = onReflectionsChanged;
            falloffStrength.onValueChanged = onReflectionsChanged;
            falloffStart.onValueChanged = onReflectionsChanged;
            enableFalloff.onValueChanged = onReflectionsChanged;
            scrollingStrength.onValueChanged = onReflectionsChanged;
            enableScrolling.onValueChanged = onReflectionsChanged;
            enableTopDownReflections.onValueChanged = onReflectionsChanged;
            enablePlatformerReflections.onValueChanged = onReflectionsChanged;
            DistortionFPRH.onValueChanged = onReflectionsChanged;
        }
    }

}