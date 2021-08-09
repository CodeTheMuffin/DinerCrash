using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip deniedUIClip;// For when the player can't access the UI menu
    public AudioClip puttingOrderDown; //For when the player places an order down on counter ONLY
    public AudioClip pickingOrderUp; //For when the player picks up an order
    public AudioClip throwOrderAway; //For when the player throws away an order in the trash

    private void Start()
    {
        // you need a reference to your component
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    /*
     Method/Function Naming:
        play{where from or by who}{Action}
     */
    public void playUI_denied()
    { audioSource.PlayOneShot(deniedUIClip); }

    public void playPlayerPutOrderOnCounter()
    { audioSource.PlayOneShot(puttingOrderDown); }

    public void playPlayerPickOrderUp()
    { audioSource.PlayOneShot(pickingOrderUp); }
    public void playPlayerThrowOrderAway()
    { audioSource.PlayOneShot(throwOrderAway); }
}
