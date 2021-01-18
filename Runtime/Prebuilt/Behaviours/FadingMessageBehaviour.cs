using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VanillaWorks.MessageLogger.Runtime.Core.Behaviours;
using VanillaWorks.MessageLogger.Runtime.Core.Models;
using VanillaWorks.MessageLogger.Runtime.Prebuilt.Models;

namespace VanillaWorks.MessageLogger.Runtime.Prebuilt.Behaviours
{
    public class FadingMessageBehaviour : MessageBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected new FadingMessage messageData => base.messageData as FadingMessage;
        
        [Header("Sub Class Fields")]
        [SerializeField]
        private bool _keepAliveOnPointerOver;
        private Color _defaultTextColor;

        public override void Initialize()
        {
            base.Initialize();
            _defaultTextColor = text.color;
        }

        public override void Activate(MessageData data, Action<MessageBehaviour> dispose)
        {
            base.Activate(data, dispose);
            messageData.Activate();
            text.text = messageData.MessageText;
            
        }

        public override void Dispose()
        {
            text.color = _defaultTextColor;
            base.Dispose();
        }

        protected virtual void Update()
        {
            text.color = messageData.TextColor;
            if (messageData.IsExpired)
            {
               Dispose();
            }
        }
        
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if(!_keepAliveOnPointerOver) return;
            messageData.StopTimer();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if(!_keepAliveOnPointerOver) return;;
            messageData.ResumeTimer();
        }
    }
}