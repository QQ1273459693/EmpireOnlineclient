using GabrielBigardi.SpriteAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DoorInteractableController : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteAnimator DoorAnimator;
    public SpriteAnimator TopAnimator;
    public Light2D m_RoomLight;
    public float LightValue;
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"有人进来了{other.name}");
        if (other != null && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(LightBackMove());
            StartCoroutine(LightMove());
            DoorAnimator.Play("Open");
            TopAnimator.Play("Open");
        }
    }
    IEnumerator LightMove()
    {
        while (m_RoomLight.intensity < LightValue)
        {
            m_RoomLight.intensity += 0.03F;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator LightBackMove()
    {
        while (m_RoomLight.intensity > 0F)
        {
            m_RoomLight.intensity -= 0.03F;
            yield return new WaitForEndOfFrame();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"有人退出了{other.name}");
        if (other != null && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(LightMove());
            StartCoroutine(LightBackMove());
            DoorAnimator.Play("Close");
            TopAnimator.Play("Close");
        }
    }
}
