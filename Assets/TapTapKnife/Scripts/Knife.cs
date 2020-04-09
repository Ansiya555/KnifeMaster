using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knife : MonoBehaviour
{
    Animator animator;
    bool OnBoard = false;
    float duration=.1f;
    float magnitude=1f;
  //  bool collected;



    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains(TapTapKnife.boardString)&&!OnBoard)
        {
            OnBoard = true;
          //  print(GetInstanceID());
          /*  if(!TapTapKnife.instance.knifeHitKnifeSFX.isPlaying)
            {
                TapTapKnife.instance.knifeHitBoardSFX.Play();
            }*/
            TapTapKnife.instance.UpdateScore(TapTapKnife.instance.knifeHitPoints);
            GameObject boardVFX = Instantiate(TapTapKnife.instance.boardHitVFX, TapTapKnife.instance.KnifehitparticlePos,Quaternion.EulerAngles(new Vector3(0f,90f,-90f)));
            Destroy(boardVFX, 1f);
            Destroy(this,.25f);
         
        }
        
      if (other.gameObject.name.Contains(TapTapKnife.knifeString))
        {
            //print("HitKnifeHit   "+GetInstanceID());
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            
            TapTapKnife.isGameOver = true;
            TapTapKnife.instance.StopAllCoroutines();
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
            GameObject collectibleVFX = Instantiate(TapTapKnife.instance.collectibleVFX, other.gameObject.transform.position, Quaternion.identity);
            Destroy(collectibleVFX, 1f);
            //ToDo: VFX and Update Score
            //   TapTapKnife.instance.plusFifty.transform.position = other.gameObject.transform.position;
            RectTransform CanvasRect = TapTapKnife.instance.canvas.GetComponent<RectTransform>();
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(other.gameObject.transform.position);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element
         //   TapTapKnife.instance.plusFifty..anchoredPosition = WorldObject_ScreenPosition;






         //   StartCoroutine(KnifeRepositioning(other.gameObject.transform.localPosition, other.gameObject.transform.localRotation.eulerAngles));
            
            Destroy(other.gameObject);
            TapTapKnife.instance.UIEnabler(WorldObject_ScreenPosition);

        }
    }

 /* IEnumerator KnifeRepositioning(Vector3 newPos,Vector3 rotation)
    {
        yield return new WaitForSeconds(.05f);
        print(transform.position);
        print(newPos);
        transform.localPosition = newPos;
        rotation.z = rotation.z + 180f;
        transform.localRotation = Quaternion.Euler(rotation);
        var fsfsdf = transform.parent;
        transform.parent = null;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y+.075f, transform.localPosition.z);
        transform.parent = fsfsdf;
        // transform.localRotation = Quaternion.Euler(0f,0f,rotation.z + 180f);
        //  transform.localRotation= new Quaternion(transform.localRotation.x, transform.localRotation.y, (transform.localRotation.z), transform.localRotation.w);
        Destroy(this);
    }

    */

  
}
