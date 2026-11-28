using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicChangeTrigger: MonoBehaviour
{
    [SerializeField] private EventReference music;
    [SerializeField] private bool isOneTime = true;
   

    private bool isUsed = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOneTime && isUsed)
            return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Change Music");
            
            EventInstance newMusic = RuntimeManager.CreateInstance(music);
            EventInstance currentMusic = EnvironmentDirector.Instance.GetCurrentMusic();
            currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            newMusic.start();
            EnvironmentDirector.Instance.SetMusic(newMusic);
            isUsed = true;
        }
    }
}
