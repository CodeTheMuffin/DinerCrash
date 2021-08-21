using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    AudioSource BackgroundAudioSource;
    public AudioClip BackgroundMusic; //play on loop
    public AudioClip deniedUIClip;// For when the player can't access the UI menu
    public AudioClip puttingOrderDown; //For when the player places an order down on counter ONLY
    public AudioClip pickingOrderUp; //For when the player picks up an order
    public AudioClip throwOrderAway; //For when the player throws away an order in the trash
    public AudioClip orderReadyForPickUp; // For when the order is done being processed
    public AudioClip NPCwalkingInside; // For when the NPC is walking into the building
    public AudioClip NPCreadyToOrder; // For when the NPC is ready to order
    public AudioClip okayOrder; // When NPC receives an okay order
    public AudioClip badOrder; // When NPC receives an bad order
    public AudioClip goodOrder; // When NPC receives a good order
    public AudioClip missedOrder; // When NPC leaves and didn't receive an order

    private void Start()
    {
        // you need a reference to your component
        audioSource = gameObject.GetComponents<AudioSource>()[0];
        BackgroundAudioSource = gameObject.GetComponents<AudioSource>()[1];
        playBackgroundMusic();
    }

    /*
     Method/Function Naming:
        play{where from or by who}{Action}
     */

    public void playBackgroundMusic()
    {
        BackgroundAudioSource.clip = BackgroundMusic;
        BackgroundAudioSource.loop = true;
        BackgroundAudioSource.volume = 0.4f;
        BackgroundAudioSource.Play();
    }

    public void playUI_denied()
    { audioSource.PlayOneShot(deniedUIClip); }

    public void playPlayerPutOrderOnCounter()
    { audioSource.PlayOneShot(puttingOrderDown); }

    public void playPlayerPickOrderUp()
    { audioSource.PlayOneShot(pickingOrderUp); }
    public void playPlayerThrowOrderAway()
    { audioSource.PlayOneShot(throwOrderAway, 1f); }

    public void playOrderReadyForPickUp()
    { audioSource.PlayOneShot(orderReadyForPickUp, 0.90f); }

    public void playNPCwalkingInside()
    { audioSource.PlayOneShot(NPCwalkingInside); }

    public void playNPCreadyToOrder()
    { audioSource.PlayOneShot(NPCreadyToOrder); }

    public void playOkayOrder()
    { audioSource.PlayOneShot(okayOrder); }

    public void playBadOrder()
    { audioSource.PlayOneShot(badOrder); }

    public void playGoodOrder()
    { audioSource.PlayOneShot(goodOrder); }

    public void playMissedOrder()
    { audioSource.PlayOneShot(missedOrder); }
}
