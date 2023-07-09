using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BuffTextItem : MonoBehaviour
{
    public Text text;
    public CanvasGroup canvasGroup;
    public void PlayAnim(BuffConfig buffcfg)
    {
        text.text = buffcfg.buffName ;
        gameObject.SetActive(false);
        float endY = transform.localPosition.y + 100;
        transform.DOLocalMoveY(endY,1f) .SetDelay(0.7f).OnStart(()=> { gameObject.SetActive(true); }).OnComplete(()=> {
            
        });
        transform.DOScale(0, 0.6f).SetDelay(1.3f) .OnComplete(() => {
            Destroy(gameObject);
        });
    }
    public void PlaybuffDamageAnim(BuffConfig buffcfg)
    {
        text.text = buffcfg.buffName + "伤害"  ;
    
        float endY = transform.localPosition.y + 100;
        transform.DOLocalMoveY(endY, 1f);
        canvasGroup.DOFade(0, 0.5f).SetDelay(1.2f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
