using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorScript : MonoBehaviour
{
    public GameObject Indicator;
    public GameObject Target;
    Renderer rd;
    public LayerMask cambox;

    public CharacterItemInteractions itemInteractions;
    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!itemInteractions.holdingItem)
        {
            if(rd.isVisible == false)
            {
                if(Indicator.activeSelf == false)
                {
                    Indicator.SetActive(true);
                }

                Vector2 direction = Target.transform.position - transform.position;

                RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, float.PositiveInfinity, cambox);

                if(ray.collider != null)
                {   
                    Indicator.transform.position = ray.point;
                }
            }
            else
            {
                if(Indicator.activeSelf == true)
                {
                    Indicator.SetActive(false);
                }
            }
        }
    }
}
