using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Attributes;
using VanillaWorks.MessageLogger.Runtime.Core.Behaviours;
using VanillaWorks.MessageLogger.Runtime.Core.Models;
using VanillaWorks.MessageLogger.Runtime.Core.Services;

namespace VanillaWorks.MessageLogger.Runtime.Core.Controllers
{
    public class MessagesControllerBase : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _useCustomMessageBehaviour = false;

        [Tooltip("Do you want limit your message count on screen")] [SerializeField]
        private bool _limitMessagesOnScreen = false;

        [ConditionalHide("_limitMessagesOnScreen", true)] [SerializeField]
        private int _maxMessageOnScreen = 5;

        [Space] [SerializeField] private bool _useThisObjectAsParent = false;

        [ConditionalHide("_useThisObjectAsParent", true, true)] [SerializeField]
        private Transform _parentObject;

        [Space] [Header("Pool Options")] [SerializeField]
        protected int prePoolCount = 5;

        [Tooltip(
            "Object which has Text component in self or one of its children." +
            "\n This will be used to display messages on screen")]
        [SerializeField]
        protected GameObject messagePrefab;

        #endregion

        #region Private And Protected Fields

        private Type _customMessageBehaviour;

        protected PoolService poolService;

        protected Queue<MessageData> messagesQueue = new Queue<MessageData>();

        protected List<MessageBehaviour> activeMessages = new List<MessageBehaviour>();
        protected bool CanShowMessage => !_limitMessagesOnScreen || activeMessages.Count <= _maxMessageOnScreen;
        protected Transform ParentTransform => _useThisObjectAsParent ? transform : _parentObject;

        protected Type BehaviourType =>
            _useCustomMessageBehaviour
                ? _customMessageBehaviour
                : typeof(MessageBehaviour);

        #endregion

        #region Public Fields

        public int ActiveMessagesCount => activeMessages.Count;

        #endregion

        #region Actions

        public virtual event Action<MessageBehaviour> MessageActivated;

        #endregion


        protected virtual void Awake()
        {
            MessageData.messagesController = this;
            CheckForCustomBehaviour();
            poolService = new PoolService(prePoolCount, BehaviourType, messagePrefab, ParentTransform);
        }

        protected virtual void OnDestroy()
        {
            DisposeController();
        }

        public virtual void DisplayMessage(MessageData messageData)
        {
            if (messageData is null)
                return;
            messagesQueue.Enqueue(messageData);
            ActivateMessage();
        }

        public virtual void DisposeMessage(MessageBehaviour behaviour)
        {
            if (!behaviour) return;
            activeMessages.Remove(behaviour);
            poolService.ReturnToPool(behaviour);
            ActivateMessage();
        }

        public virtual void DisposeMessage(MessageData data)
        {
            var behaviour = activeMessages.Find(x => x.MessageData == data && x.IsActive);
            if (!behaviour)
            {
                //if there is no active behaviour carrying this particular data then we should remove it from queue too
                if (messagesQueue.Contains(data))
                {
                    messagesQueue = new Queue<MessageData>(messagesQueue.Where(x => x != data));
                }

                return;
            }

            behaviour.Dispose();
        }

        protected virtual void ActivateMessage()
        {
            if (messagesQueue.Count < 1 || !CanShowMessage) return;
            var messageBehaviour = poolService.GetFromPool();
            Activate(messagesQueue.Dequeue(), messageBehaviour);
        }

        protected virtual void Activate(MessageData data, MessageBehaviour behaviour)
        {
            data.Behaviour = behaviour;
            activeMessages.Add(behaviour);
            behaviour.Activate(data, DisposeMessage);
            MessageActivated?.Invoke(behaviour);
        }

        protected virtual void DisposeController()
        {
            poolService.Dispose();
        }
        private void CheckForCustomBehaviour()
        {
            if (!_useCustomMessageBehaviour) return;
            var customMessageBehaviour = messagePrefab.GetComponentInChildren<MessageBehaviour>(true);
            if (customMessageBehaviour)
            {
                _customMessageBehaviour = customMessageBehaviour.GetType();
            }
            else
            {
                _customMessageBehaviour = typeof(MessageBehaviour);
                Debug.LogError(
                    "Custom message behaviour not found in message prefab, message controller will use default Message Behaviour");
            }
        }
    }
}