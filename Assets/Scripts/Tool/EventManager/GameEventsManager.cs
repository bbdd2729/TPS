using System;
using System.Collections.Generic;
using GGG.Tool;
using GGG.Tool.Singleton;

public class GameEventsManager : SingletonNonMono<GameEventsManager>
{
   //дһ���ֵ����������������༰�¼�����̬��
   private Dictionary<string, IEventface> EventCenters = new Dictionary<string, IEventface>();

   //ע���¼���EventHander(action)д���ֵ�������Ӷ���ص�
   public void AddEventListening(string name, Action action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander)?.AddCallBack(action);
      }
      else
      {
         EventCenters.Add(name, new EventHander(action));
      }
   }

   public void AddEventListening<T>(string name, Action<T> action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T>)?.AddCallBack(action);
      }
      else
      {
         EventCenters.Add(name, new EventHander<T>(action));
      }
   }

   public void AddEventListening<T1, T2>(string name, Action<T1, T2> action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T1, T2>)?.AddCallBack(action);
      }
      else
      {
         EventCenters.Add(name, new EventHander<T1, T2>(action));
      }
   }

   public void AddEventListening<T1, T2, T3, T4, T5, T6>(string name, Action<T1, T2, T3, T4, T5, T6> action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T1, T2, T3, T4, T5, T6>)?.AddCallBack(action);
      }
      else
      {
         EventCenters.Add(name, new EventHander<T1, T2, T3, T4, T5, T6>(action));
      }
   }

   public void CallEvent(string name)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander)?.CallBack();
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   public void CallEvent<T>(string name, T value)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T>)?.CallBack(value);
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   public void CallEvent<T1, T2>(string name, T1 t1, T2 t2)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T1, T2>)?.CallBack(t1, t2);
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   public void CallEvent<T1, T2, T3, T4, T5, T6>(string name, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T1, T2, T3, T4, T5, T6>)?.CallBack(t1, t2, t3, t4, t5, t6);
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   public void ReMoveEvent(string name, Action action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander)?.RemoveCallBack(action);
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   public void ReMoveEvent<T1>(string name, Action<T1> action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T1>)?.RemoveCallBack(action);
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   public void ReMoveEvent<T1, T2>(string name, Action<T1, T2> action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T1, T2>)?.RemoveCallBack(action);
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   public void ReMoveEvent<T1, T2, T3, T4, T5, T6>(string name, Action<T1, T2, T3, T4, T5, T6> action)
   {
      if (EventCenters.TryGetValue(name, out var e))
      {
         (e as EventHander<T1, T2, T3, T4, T5, T6>)?.RemoveCallBack(action);
      }
      else
      {
         DevelopmentToos.WTF("��ǰ�¼������ֵ��ڲ�����");
      }
   }

   private interface IEventface
   {
   }

   private class EventHander : IEventface
   {
      //�ṩ���ķ���
      public EventHander(Action action)
      {
         _action = action;
      }

      //�����¼�
      private event Action _action;

      public void AddCallBack(Action action)
      {
         _action += action;
      }

      public void RemoveCallBack(Action action)
      {
         _action -= action;
      }

      public void CallBack()
      {
         _action?.Invoke();
      }
   }

   //���캯�������ֶγ�ʼ�������ҪΪ����
   private class EventHander<T> : IEventface
   {
      //�ṩ���ķ���
      public EventHander(Action<T> action)
      {
         _action = action;
      }

      //�����¼�
      private event Action<T> _action;

      public void AddCallBack(Action<T> action)
      {
         _action += action;
      }

      public void RemoveCallBack(Action<T> action)
      {
         _action -= action;
      }

      public void CallBack(T value)
      {
         _action?.Invoke(value);
      }
   }

   private class EventHander<T1, T2> : IEventface
   {
      //�ṩ���ķ���
      public EventHander(Action<T1, T2> action)
      {
         _action = action;
      }

      //�����¼�
      private event Action<T1, T2> _action;

      public void AddCallBack(Action<T1, T2> action)
      {
         _action += action;
      }

      public void RemoveCallBack(Action<T1, T2> action)
      {
         _action -= action;
      }

      public void CallBack(T1 t1, T2 t2)
      {
         _action?.Invoke(t1, t2);
      }
   }

   private class EventHander<T1, T2, T3, T4, T5, T6> : IEventface
   {
      //�ṩ���ķ���
      public EventHander(Action<T1, T2, T3, T4, T5, T6> action)
      {
         _action = action;
      }

      //�����¼�
      private event Action<T1, T2, T3, T4, T5, T6> _action;

      public void AddCallBack(Action<T1, T2, T3, T4, T5, T6> action)
      {
         _action += action;
      }

      public void RemoveCallBack(Action<T1, T2, T3, T4, T5, T6> action)
      {
         _action -= action;
      }

      public void CallBack(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
      {
         _action?.Invoke(t1, t2, t3, t4, t5, t6);
      }
   }
}