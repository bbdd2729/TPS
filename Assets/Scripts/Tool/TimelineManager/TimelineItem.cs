using UnityEngine;
using UnityEngine.Playables;

public class TimelineItem : MonoBehaviour
{
   //public PlayableDirector PlayableDirector;

   public void PlayTimeline(PlayableDirector playableDirector)
   {
      playableDirector.Play();
   }
}