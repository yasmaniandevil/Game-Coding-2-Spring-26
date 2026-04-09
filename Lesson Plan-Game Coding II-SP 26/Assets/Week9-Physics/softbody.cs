using UnityEngine;

//forces the game object to always have a skinned mesh renderer 
//if it doesnt unity attaches one
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class softbody : MonoBehaviour
{
    [Range(0, 2f)] //how far veticies can move (higher = more floppy)
    public float softness = 1;

    //how much motion slows down (like friction)
    [Range(0.01f, 1f)] public float damping = 0.1f;

    //how resistent it is to bending
    public float stiffness = 1f;
    
    //if you want this script to be an interactable object add physicsobj.cs and interactable
    //make sure the mass is low
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //game starts, builds it automatically
        CreateSoftBodyPhysics();
    }
   

    void CreateSoftBodyPhysics()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.Log("skinned mesh renderer is null");
            return;
        }
        //adds unity cloth physics comp to obj at runtime
        Cloth cloth = gameObject.AddComponent<Cloth>();
        cloth.damping = damping;
        cloth.bendingStiffness = stiffness;
        
        //every vertex in the mesh gets a physics rule
        //we generate the rules with our function
        cloth.coefficients = 
            GenerateClothCoefficents(smr.sharedMesh.vertices.Length);
    }

    
    //we are making an array so we have multiple coeeficents for all verticies
    //ex: mesh might have 500 vertecies so cloth needs 500 coefficents (one per vertex)
    //so thats why we are returning an array
    private ClothSkinningCoefficient[] GenerateClothCoefficents(int vertexCount)
    {
        //[] creates an array one entry per vertex
        //make a list with vertexcount slots
        ClothSkinningCoefficient[] coefficients = new ClothSkinningCoefficient[vertexCount];

        //loop thru every vertex
        //set rules for each vertex 1 by 1
        for (int i = 0; i < vertexCount; i++)
        {
            //how far that vertex can move
            coefficients[i].maxDistance = softness;
            //collision buffer 0 = tight
            coefficients[i].collisionSphereDistance = 0f;
            //so basically every vertex can move up to softness distance
        }
        //send it back to the cloth component
        return coefficients;
    }
}
