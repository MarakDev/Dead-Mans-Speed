using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static GameManager;

public class LightController : MonoBehaviour
{
    [SerializeField] public float fullLightsSteps = 0.2f;
    [SerializeField] public float thunderLightsSteps = 0.1f;
    private float maxLight = 1.2f;
    [HideInInspector] public Light2D lights;

    public void Start()
    {
        lights = GetComponent<Light2D>();

        lights.intensity = 0;

    }


    public IEnumerator TurnOffLights()
    {
        while (lights.intensity > 0.01f)
        {
            yield return new WaitForSeconds(0.75f);
            lights.intensity -= fullLightsSteps;
            AudioManager.instance.PlayWithPitch(AudioManager.instance.sound_Time_Bar, 0.3f);
        }

        lights.intensity = 0;

        AudioManager.instance.StopMusic();

        yield return new WaitForSeconds(0.5f);

        GameManager.instance.STARTGAME = true;
        GameManager.instance.mainMenuIntro.SetActive(true);


    }

    public IEnumerator TurnOnLights()
    {
        while (lights.intensity < maxLight - 0.01f)
        {
            yield return new WaitForSeconds(0.75f);
            lights.intensity += fullLightsSteps;
            AudioManager.instance.PlayWithPitch(AudioManager.instance.sound_Time_Bar, 0.4f);
        }

        lights.intensity = maxLight;

        yield return new WaitForSeconds(0.25f);

        GameManager.instance.arbitroDecoration.GetComponent<Animator>().Play("Arbitro_GetUp");
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Cavar, 0.75f);


        yield return new WaitForSeconds(0.5f);

        GameManager.instance.currentState = GameManager.GameState.WaitForStartRound;


    }

    public IEnumerator ThunderLight()
    {
        yield return new WaitForSeconds(thunderLightsSteps / 2);

        lights.intensity = 15;

        yield return new WaitForSeconds(thunderLightsSteps);

        lights.intensity = 0;

        yield return new WaitForSeconds(thunderLightsSteps);

        lights.intensity = 15;

        yield return new WaitForSeconds(thunderLightsSteps);

        lights.intensity = maxLight;
    }
}
