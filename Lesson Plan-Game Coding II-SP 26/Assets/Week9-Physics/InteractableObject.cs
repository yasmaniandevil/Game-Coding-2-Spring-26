using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Highlight Settings")]
    //the color the object glows when the player looks at it
    public Color highlightColor = new Color(1f, 0.95f, 0.6f); //soft warm yellow
    //how strongly the highlight color blenders with the original,0 = no effect, 1 = full replace
    [Range(0f, 1f)]
    public float highlightStrength = .4f;

    public Renderer objectRenderer; //the render component on this object
    Color originalColor; //the color before any highlight was applied
    bool isHighlighted = false; //are we currently highlighted

    private void Awake()
    {
        if(objectRenderer == null)
        {
            //cache the renderer so we are not calling the getcomp every frame
            objectRenderer = gameObject.GetComponent<Renderer>();
        }
        
        if (objectRenderer != null)
        {
            //store the original color so we can restore it after unhighlighting
            //we read from the materials color property
            //we use sharedmaterial to read the base color without instancing
            originalColor = objectRenderer.sharedMaterial.color;

        }
        else
        {
            Debug.Log($"Interactable object on {gameObject.name} has no rendered. highlight wont work");
        }
    }
   

    public void Highlight()
    {
        if (isHighlighted || objectRenderer == null)
        {
            //Debug.Log("no object renderer & ishighlighted is true");
            return;
        }
        //color.lerp blends btwn the orignal color and highlight color
        //by the highlight strength amt 
        //we use material not shared material to create a unique instance
        //so we dont affect every object using the same material
        objectRenderer.material.color = Color.Lerp(originalColor, highlightColor, highlightStrength);
        isHighlighted = true;
    }

    //called by object gravver when the player looks away
    //restores original color
    public void Unhighlight()
    {
        if (!isHighlighted || objectRenderer == null) return;

        objectRenderer.material.color = originalColor;
        isHighlighted = false;
    }
}
