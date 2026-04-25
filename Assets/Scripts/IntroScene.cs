using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    private string firstSongName = "Main_1";
    private string secondSongName = "Main_2";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BGMCorountine());
    }
    private IEnumerator BGMCorountine()
    {
        AudioController.Instance.SetLoopAndPlay(firstSongName, looping: false);
        // 2. 获取这首歌的长度
        AudioClip currentClip = AudioController.Instance.GetLoopClip(firstSongName); 
        float fadeTime = 2f; // 提前2秒开始切歌
        if (currentClip != null)
        {
            // 等待这首歌的播放时间
            
            yield return new WaitForSeconds(currentClip.length - fadeTime);
        }
        else
        {
            // 如果没找到这首歌，为了防止死循环卡死，随便等 1 秒钟跳过
            Debug.LogWarning("找不到音乐: " + firstSongName);
            yield return new WaitForSeconds(1f);
        }
        // 提前两秒调用交叉淡入淡出，切到下一首歌
        //AudioController.Instance.fade
        AudioController.Instance.CrossFadeLoop(secondSongName, fadeTime);
    }
}
