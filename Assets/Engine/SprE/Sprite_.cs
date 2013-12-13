using UnityEngine;
using System.Collections;
 

public delegate void OnLastFrame();
class Sprite_ : MonoBehaviour
{
    public int width=128,height=128,image_amount=1;
    public float framesPerSecond = 10f;
 
	int main_w,main_h,columns,rows;
	float w_ratio,h_ratio;
    //the current frame to display
    private int index = 0;
 
	public OnLastFrame on_last;
	
    void Start()
    {
		//start from top
		renderer.material.SetTextureOffset("_MainTex", new Vector2(0f,1f));
		
		main_w=renderer.material.mainTexture.width;
		main_h=renderer.material.mainTexture.height;
		
		columns=(int)((float)main_w/width);
		rows=(int)((float)main_h/height);
		
		w_ratio=1f / columns;
		h_ratio=1f / rows;
		
        //set the tile size of the texture (in UV units), based on the rows and columns
        Vector2 size = new Vector2(w_ratio,h_ratio);
        renderer.material.SetTextureScale("_MainTex", size);
		
		StartCoroutine(updateTiling());
    }
 
    private IEnumerator updateTiling()
    {
        while (true)
        {
            //split into x and y indexes
            Vector2 offset = new Vector2(index%columns*w_ratio,(1+(index/columns))*-h_ratio);
            renderer.material.SetTextureOffset("_MainTex", offset);
			
            //move to the next index
            index++;
            if (index >= image_amount){
                index = 0;
				if (on_last!=null) on_last();
			}
			
			yield return new WaitForSeconds(1f / framesPerSecond);
		}
 
    }
}