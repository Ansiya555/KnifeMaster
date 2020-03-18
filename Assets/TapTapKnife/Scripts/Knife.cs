﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knife : MonoBehaviour
{
    Animator animator;
    bool OnBoard = false;
    float duration=.1f;
    float magnitude=1f;




    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains(TapTapKnife.boardString)&&!OnBoard)
        {
            OnBoard = true;
            print("coll");
          /*  if(!TapTapKnife.instance.knifeHitKnifeSFX.isPlaying)
            {
                TapTapKnife.instance.knifeHitBoardSFX.Play();
            }*/
            TapTapKnife.instance.UpdateScore(TapTapKnife.instance.knifeHitPoints);
            GameObject boardVFX = Instantiate(TapTapKnife.instance.boardHitVFX, TapTapKnife.instance.particlePos,Quaternion.identity);
            Destroy(boardVFX, 1f);
            Destroy(this);
        }

        if (other.gameObject.name.Contains(TapTapKnife.knifeString))
        {
            print("HitKnifeHit   "+GetInstanceID());
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            TapTapKnife.instance.knifeHitBoardSFX.Stop();
            TapTapKnife.instance.knifeHitKnifeSFX.Play();
            TapTapKnife.isGameOver = true;
            gameObject.transform.parent = null;
            animator.enabled = true;
            GameObject knifeHitKnifeVFX = Instantiate(TapTapKnife.instance.knifeHitKnifeVfx, TapTapKnife.instance.particlePos, Quaternion.identity);
            Destroy(knifeHitKnifeVFX, 1f);
            TapTapKnife.instance.redPanelEnabler();
        }

        if (other.gameObject.name.Contains(TapTapKnife.collectibleString))
        {
            TapTapKnife.instance.collectibleSFX.Play();
            TapTapKnife.instance.UpdateScore(TapTapKnife.instance.collectiblesPoints - TapTapKnife.instance.knifeHitPoints);
            GameObject collectibleVFX = Instantiate(TapTapKnife.instance.collectibleVFX, TapTapKnife.instance.particlePos, Quaternion.identity);
            Destroy(collectibleVFX, 1f);
            //ToDo: VFX and Update Score
            Destroy(other.gameObject);
        }
    }

   



  
}
