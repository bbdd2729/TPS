namespace ZZZ
{
   public abstract class StateMachine
   {
      //����һ���̳���BindableProperty���͵�IState�ֶΣ���ȡIstateҪͨ��.Value
      public BindableProperty<IState> currentState = new BindableProperty<IState>();

      /// <summary>
      /// �л�״̬�Ľӿ�API
      /// </summary>
      /// <param name="newState"></param>
      public void ChangeState(IState newState)
      {
         //����Ϊ���ã��߼���
         currentState.Value?.Exit();

         currentState.Value = newState;

         currentState.Value.Enter();
      }

      /// <summary>
      /// ��������Ľӿ�API
      /// </summary>
      public void HandInput()
      {
         //ֻ����һ��״̬���������
         currentState.Value?.HandInput();
      }

      /// <summary>
      /// ���·������߼��Ľӿ�API
      /// </summary>
      public void Update()
      {
         currentState.Value?.Update();
      }

      /// <summary>
      /// ִ�ж����¼��Ľӿ�API
      /// </summary>
      public void OnAnimationTranslateEvent(IState translateState)
      {
         currentState.Value?.OnAnimationTranslateEvent(translateState);
      }

      public void OnAnimationExitEvent()
      {
         currentState.Value?.OnAnimationExitEvent();
      }
   }
}