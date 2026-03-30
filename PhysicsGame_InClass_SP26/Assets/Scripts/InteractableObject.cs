using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //the color the object glows when the player looks at it
    public Color highlightColor = new Color(1f, 0.95f, 0.6f);
    //how strong the highlight color blends with the original color 0= no effect 1 = full replace
    [Range(0, 1f)] public float highlightStrength = .4f;

    private Renderer objectRenderer; //the render comp on this obj
    private Color originalColor; //the color b4 any highlight was applied
    private bool isHighlighted = false; //are we currently highlighted?
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        objectRenderer = GetComponent<Renderer>(); //cache the renderer so we are not calling get comp every frame
        if (objectRenderer != null)
        {
            //store the original color so we can restore it after unhighlighting
            //we read from the materials color property
            //we use sharedmaterial to read the base color without instancing
            originalColor = objectRenderer.sharedMaterial.color;
        }
        else
        {
            Debug.Log($"Interactable object on {gameObject.name} has no renderer. highlight wont work");
            
        }
    }

    public void Highlight()
    {
        if (isHighlighted || objectRenderer == null)
        {
            Debug.Log("no obj renderer & ishighlighted is true");
            return;
        }
        
        //color.lerp blends betn the original color and the highlighted color
        //by the highlight strength amt
        //we use material not shared material to create a unique instance here
        //so we dont effect every obj using the same material
        objectRenderer.material.color = Color.Lerp(originalColor, highlightColor, highlightStrength);
        isHighlighted = true;
    }

    //called by object grabber when the player looks away
    //restores original color
    public void Unhighlight()
    {
        if (!isHighlighted || objectRenderer == null) return;
        objectRenderer.material.color = originalColor;
        isHighlighted = false;
    }

}
