using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoController : MonoBehaviour
{
    public Animator logoAnimator; // Reference to the Animator
    // Start is called before the first frame update
    void Start()
    {
        // Start the fade-out animation after a delay (e.g., 2 seconds)
        StartCoroutine(StartFadeOut());
    }

    private IEnumerator StartFadeOut()
    {
        // Wait for 2 second before starting the fade-out
        yield return new WaitForSeconds(2f);

        // Trigger the fade-out animation
        logoAnimator.Play("FadeOut");
        // Load the MenuScene
        SceneManager.LoadScene("MenuScene");
    }
}