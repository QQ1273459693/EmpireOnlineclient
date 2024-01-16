using LeafUtilsRays;
using UnityEngine;

namespace LightRays2D
{

    public static class RayParticlesUtils
    {
        public static void SetParticlesSettings(ParticleSystem system, RayParticlesType type, Transform raysTransform, EditableSpriteQuad spriteQuad, RaysParticlesSettings settings)
        {
            system.transform.ResetLocaly();
            SetParticlesLook(system, settings);
            SetParticlesPosition(system, type, raysTransform, spriteQuad, settings);

        }

        private static void SetParticlesPosition(ParticleSystem system, RayParticlesType type, Transform raysTransform, EditableSpriteQuad spriteQuad, RaysParticlesSettings settings)
        {
            switch (type)
            {
                case RayParticlesType.surface:
                    SetSurfaceParticles(system, raysTransform, spriteQuad, settings); return;
                case RayParticlesType.surfaceClimbing:
                    SetSurfaceClimbingParticles(system, raysTransform, spriteQuad, settings); return;
                case RayParticlesType.topFalling:
                    SetTopFalling(system, raysTransform, spriteQuad, settings); return;
                case RayParticlesType.bottomClimbing:
                    SetBottomFalling(system, raysTransform, spriteQuad, settings); return;
            }
        }

        private static string RayParticleTypePath(RayParticlesType type)
        {
            switch (type)
            {
                case RayParticlesType.surface:
                    return "Particle Systems/surface";
                case RayParticlesType.surfaceClimbing:
                    return "Particle Systems/surface climbing";
                case RayParticlesType.topFalling:
                    return "Particle Systems/top falling";
                case RayParticlesType.bottomClimbing:
                    return "Particle Systems/bottom climbing";
            }
            return "Particle Systems/surface";
        }

        private static void ResetLocaly(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        private static void SetSurfaceParticles(ParticleSystem system, Transform raysTransform, EditableSpriteQuad spriteQuad, RaysParticlesSettings settings)
        {
            // vertBottomLeft vertBottomRight vertTopRight vertTopLeft
            Vector3[] localPosScaled = GetVertsScaled(spriteQuad, raysTransform);

            float offsetY = (-settings.topMargin.value / 2) + (settings.bottomMargin.value / 2);
            float scaleY = 1 - ((settings.topMargin.value / 2) + (settings.bottomMargin.value / 2));
            float offsetX = (settings.rightMargin.value / 2) + (-settings.leftMargin.value / 2);
            float scaleX = 1 - ((settings.rightMargin.value / 2) + (settings.leftMargin.value / 2));

            var shapeModule = system.shape;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            shapeModule.shapeType = ParticleSystemShapeType.MeshRenderer;
            shapeModule.meshRenderer = raysTransform.GetComponent<MeshRenderer>();

            shapeModule.position = new Vector3(offsetX, offsetY);
            shapeModule.scale = new Vector3(scaleX, scaleY);
        }

        private static void SetSurfaceClimbingParticles(ParticleSystem system, Transform raysTransform, EditableSpriteQuad spriteQuad, RaysParticlesSettings settings)
        {
            SetSurfaceParticles(system, raysTransform, spriteQuad, settings);
        }

        private static void SetTopFalling(ParticleSystem system, Transform raysTransform, EditableSpriteQuad spriteQuad, RaysParticlesSettings settings)
        {
            // vertBottomLeft vertBottomRight vertTopRight vertTopLeft
            Vector3[] localPosScaled = GetVertsScaled(spriteQuad, raysTransform);

            var shape = system.shape;
            shape.rotation = new Vector3(0, 0, raysTransform.rotation.eulerAngles.z);
            shape.radius = (localPosScaled[2].x - localPosScaled[3].x) * settings.width.value / 2f; ;
            float offsetY = settings.offsetY.value * (localPosScaled[2].y - localPosScaled[1].y);
            shape.position = new Vector3((localPosScaled[2].x + localPosScaled[3].x) / 2f, offsetY + ((localPosScaled[2].y + localPosScaled[3].y) / 2f));
        }

        private static void SetBottomFalling(ParticleSystem system, Transform raysTransform, EditableSpriteQuad spriteQuad, RaysParticlesSettings settings)
        {
            // vertBottomLeft vertBottomRight vertTopRight vertTopLeft
            Vector3[] localPosScaled = GetVertsScaled(spriteQuad, raysTransform);

            var shape = system.shape;
            shape.rotation = new Vector3(0, 0, raysTransform.rotation.eulerAngles.z);
            shape.radius = (localPosScaled[1].x - localPosScaled[0].x) * settings.width.value / 2f;
            float offsetY = settings.offsetY.value * (localPosScaled[2].y - localPosScaled[1].y);
            shape.position = new Vector3((localPosScaled[1].x + localPosScaled[0].x) / 2f, offsetY + ((localPosScaled[1].y + localPosScaled[0].y) / 2f));
        }

        private static Vector3[] GetVertsScaled(EditableSpriteQuad quad, Transform raysTransform)
        {
            // vertBottomLeft vertBottomRight vertTopRight vertTopLeft
            Vector3[] localPosVerts = quad.GetVerts();
            Vector3[] localPosScaled = new Vector3[localPosVerts.Length];

            //scale verts positions from mesh space to object space
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, raysTransform.rotation, raysTransform.localScale);
            for (int i = 0; i < localPosVerts.Length; i++)
            {
                localPosScaled[i] = matrix.MultiplyPoint(localPosVerts[i]);
            }
            return localPosScaled;
        }

        private static void SetParticlesLook(ParticleSystem system, RaysParticlesSettings settings)
        {
            var module = system.main;
            module.simulationSpeed = settings.speed.value;
            module.startSize = settings.size.value * 0.1f;

            var yourParticleEmission = system.emission;
            yourParticleEmission.enabled = true;
            yourParticleEmission.rateOverTime = settings.density.value * 25f;

            module.startColor = settings.color.value;
            module.startLifetime = settings.lifetime.value;
            var anim = system.textureSheetAnimation;
            anim.SetSprite(0, settings.sprite != null ? settings.sprite : anim.GetSprite(0));

            var noise = system.noise;
            noise.strength = settings.noiseStrength.value;
        }

        public static bool DeleteExistingRayParticles(Transform raysTransform)
        {
            for (int i = 0; i < 2; i++)
            {
                ParticleSystem system = raysTransform.GetComponentInChildren<ParticleSystem>();
                if (system != null)
                {
                    DestroyUtils.DestroyAlways(system.gameObject);
                    return true;
                }
            }

            return false;
        }


        public static ParticleSystem InstantiateRayParticles(RayParticlesType type, Transform raysTransform)
        {
            string path = RayParticleTypePath(type);
            GameObject prefab = (GameObject)Resources.Load(path);
            GameObject gameObject = UnityEngine.Object.Instantiate(prefab, raysTransform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.name = type.ToString();
            return gameObject.GetComponent<ParticleSystem>();
        }

    }

}