using System;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Models;
#if TMP_IMPORTED
using TMPro;
#else
using UnityEngine.UI;
#endif

namespace VanillaWorks.MessageLogger.Runtime.Core.Behaviours
{
#if TMP_IMPORTED
    [RequireComponent(typeof(TMP_Text))]
    #else
    [RequireComponent(typeof(Text))]
#endif
    public class MessageBehaviour : MonoBehaviour
    {
        protected MessageData messageData;
#if TMP_IMPORTED
        protected TMP_Text text;
#else
        protected Text text;
#endif
        private bool _isActive;
        
        protected Action<MessageBehaviour> disposeCallback;
        public bool IsActive
        {
            get => _isActive;
            protected set => _isActive = value;
        }

        public MessageData MessageData => messageData;

        public virtual void Initialize()
        {
#if TMP_IMPORTED
            text = GetComponentInChildren<TMP_Text>(true);
#else
             text = GetComponentInChildren<Text>(true);
#endif
        }

        public virtual void Activate(MessageData data, Action<MessageBehaviour> dispose)
        {
            disposeCallback = dispose;
            messageData = data;
            _isActive = true;
        }

        public virtual void Dispose()
        {
            disposeCallback?.Invoke(this);
            _isActive = false;
            messageData = null;
        }

    }
}