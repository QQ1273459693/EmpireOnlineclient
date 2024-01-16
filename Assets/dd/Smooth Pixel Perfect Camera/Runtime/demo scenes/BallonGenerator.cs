using System.Collections.Generic;
using UnityEngine;

namespace SmoothCam
{
    public class BallonGenerator : MonoBehaviour
    {

        [SerializeField] float timeToSpawn;
        [SerializeField] Vector2 SpawnCenter;
        [SerializeField] Vector2 SpawnSize;
        [SerializeField] List<string> objects;

        float timer = 0;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= timeToSpawn)
            {
                timer = 0;
                SpawnBallon();
            }
        }

        private void SpawnBallon()
        {
            GameObject toSpawn = (GameObject)Resources.Load("prefabs/" + objects[UnityEngine.Random.Range(0, objects.Count)]);
            Vector2 pos = new Vector2(UnityEngine.Random.Range(SpawnCenter.x - SpawnSize.x / 2, SpawnCenter.x + SpawnSize.x / 2), UnityEngine.Random.Range(SpawnCenter.y - SpawnSize.y / 2, SpawnCenter.y + SpawnSize.y / 2));
            Instantiate(toSpawn, pos, Quaternion.identity, transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(SpawnCenter, SpawnSize);
        }
    }

}