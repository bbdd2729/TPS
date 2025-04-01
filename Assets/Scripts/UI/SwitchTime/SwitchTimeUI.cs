using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZZZ;

public class SwitchTimeUI : MonoBehaviour, IUI
{
   [SerializeField] private Sprite corinImage;
   [SerializeField] private Sprite anBiImage;
   [SerializeField] private Sprite BiLiImage;

   [SerializeField] private Image R_Image;
   [SerializeField] private Image L_Image;

   [SerializeField] private TextMeshProUGUI TextMeshPro;
   [field: SerializeField] public float timeLeft { get; set; }
   Coroutine coroutine;
   private Vector3 initPos;
   private float millisecond = 0;
   private int millisecondInt = 0;

   private int second = 0;
   private bool stopTime = false;
   Vector3 targetPos = new Vector3(0, -640, 0);
   private RectTransform uIPos;

   private void Awake()
   {
      uIPos = GetComponent<RectTransform>();
   }

   private void Start()
   {
      initPos = uIPos.anchoredPosition;
      gameObject.SetActive(false);
   }

   private void Update()
   {
      UpdateCountDown();
   }


   public void ActiveImage(CharacterNameList LCharacterName, CharacterNameList RCharacterName, float time)
   {
      //�ȼ���
      gameObject.SetActive(true);
      //�ƶ�UI
      if (coroutine != null)
      {
         StopCoroutine(coroutine);
      }

      coroutine = StartCoroutine(MoveUI(targetPos, initPos, 10, false));

      //��ʼ��ʱ
      StartCountDown(time);

      R_Image.sprite = MatchImage(RCharacterName);

      L_Image.sprite = MatchImage(LCharacterName);
   }

   /// <summary>
   /// �Ƴ���ʾ
   /// </summary>
   public void UnActive()
   {
      if (this.gameObject.activeSelf == false)
      {
         return;
      }

      Debug.Log("�Ƴ�UI��ʾ");

      //�ƶ�UI
      if (coroutine != null)
      {
         StopCoroutine(coroutine);
      }

      coroutine = StartCoroutine(MoveUI(initPos, targetPos, 8, true));
   }

   IEnumerator MoveUI(Vector3 initPos, Vector3 targetPos, float Speed, bool canUnActive)
   {
      uIPos.anchoredPosition = initPos;
      Debug.Log("����Э��" + Vector3.Distance(targetPos, uIPos.anchoredPosition));
      while (Vector3.Distance(uIPos.anchoredPosition, targetPos) > 1f)
      {
         uIPos.anchoredPosition = Vector3.Lerp(uIPos.anchoredPosition, targetPos, Time.unscaledDeltaTime * Speed);
         yield return null;
      }

      uIPos.anchoredPosition = targetPos;

      if (canUnActive)
      {
         gameObject.SetActive(false);
      }
   }

   /// <summary>
   /// �л�ͷ��
   /// </summary>
   /// <param name="characterName"></param>
   /// <returns></returns>
   private Sprite MatchImage(CharacterNameList characterName)
   {
      switch (characterName)
      {
         case CharacterNameList.KeLin:
            return corinImage;

         case CharacterNameList.AnBi:
            return anBiImage;

         case CharacterNameList.BiLi:
            return BiLiImage;
      }

      return null;
   }

   /// <summary>
   /// ��ʼ������ʱ
   /// </summary>
   /// <param name="time"></param>
   private void StartCountDown(float time)
   {
      timeLeft = time;
      stopTime = false;
   }

   /// <summary>
   /// �����ʱ��
   /// </summary>
   private void UpdateCountDown()
   {
      if (stopTime)
      {
         return;
      }

      //������
      timeLeft -= Time.unscaledDeltaTime;
      second = Mathf.FloorToInt(timeLeft);
      //�������
      millisecond = (timeLeft - second) * 100;
      millisecondInt = Mathf.FloorToInt(millisecond);

      TextMeshPro.text = "00:" + Mathf.FloorToInt(second).ToString("00") + ":" + millisecondInt.ToString("00");

      if (second == 0 && millisecondInt == 0)
      {
         TextMeshPro.text = "00:" + Mathf.FloorToInt(timeLeft).ToString("00") + ":00";
         stopTime = true;
      }
   }
}