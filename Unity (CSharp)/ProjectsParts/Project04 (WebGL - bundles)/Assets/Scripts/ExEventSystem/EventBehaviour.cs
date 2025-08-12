using UnityEngine;
using System.Reflection;

namespace ExEvent
{
    public class EventBehaviour : MonoBehaviourBase {
        private BindingFlags m_bingingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        //protected void Subscribe(string methodName)
        //{
        //  var method = this.GetType().GetMethod(methodName, m_bingingFlags);

        //EventDispatcher.Instance.RegisterMessageHandler(this, method);        
        //}

       

        protected void Unsubscribe(MethodInfo method, ExEventHandler[] attrs)
        {
            for (int i = 0; i < attrs.Length; i++)
            {
                EventDispatcher.Instance.UnregisterMessageHandler(attrs[i].Event, this, method);
            }
        }

        protected void Unsubscribe(MethodInfo mi)
        {
            var attrs = (ExEventHandler[])mi.GetCustomAttributes(typeof(ExEventHandler), true);
            for (int i = 0; i < attrs.Length; i++)
            {
                EventDispatcher.Instance.UnregisterMessageHandler(attrs[i].Event, this, mi);
            }
        }


        protected virtual void Awake()
        {
            var methods = this.GetType().GetMethods(m_bingingFlags);

            foreach (MethodInfo mi in methods)
            {
                var attrs = (ExEventHandler[])mi.GetCustomAttributes(typeof(ExEventHandler), true);
                if (attrs.Length != 0)
                {
                    for (int i = 0; i < attrs.Length; i++)
                    {
                        EventDispatcher.Instance.RegisterMessageHandler(attrs[i].Event, attrs[i].OnlyIfEnabled, this, mi);
                    }
                }
            }
        }

        protected virtual void OnDestroy()
        {
            var methods = this.GetType().GetMethods(m_bingingFlags);

            foreach (MethodInfo mi in methods)
            {
                var attrs = (ExEventHandler[])mi.GetCustomAttributes(typeof(ExEventHandler), true);
                if (attrs.Length != 0)
                {
                    for (int i = 0; i < attrs.Length; i++)
                    {
                        Unsubscribe(mi,attrs);
                    }
                }
            }
        }
    }
}