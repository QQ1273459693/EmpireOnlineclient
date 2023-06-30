using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    public Transform skillAnim;
    public Text skillNameText;
    public Image heroIconImage;
    public void PlayAnim(SkillConfig skill,int heroid)
    {
        skillNameText.text = skill.skillName;
        heroIconImage.sprite = ResourcesManager.Instance.LoadAsset<Sprite>("Texture/"+heroid);
        skillAnim.localScale = Vector3.one;
        skillAnim.localPosition = new Vector3(340, 0,0);
        skillAnim.DOLocalMoveX(0,0.1f).OnComplete(()=> {
            skillAnim.DOLocalMoveY(10,0.5f).SetLoops(-1,LoopType.Yoyo);
        });
        skillAnim.DOLocalMoveX(340, 0.1f).SetDelay(1.5f);
    }
}
