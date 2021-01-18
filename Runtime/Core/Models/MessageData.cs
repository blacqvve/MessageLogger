using System;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Behaviours;
using VanillaWorks.MessageLogger.Runtime.Core.Controllers;

namespace VanillaWorks.MessageLogger.Runtime.Core.Models
{
    [Serializable]
    public class MessageData
    {
        public static MessagesControllerBase messagesController;
        [SerializeField] protected string messageText;

        [SerializeField] protected MessageBehaviour behaviour;
       
        public string MessageText => messageText;

        public MessageBehaviour Behaviour
        {
            get => behaviour;
            set
            {
                if (value is null)
                    return;
                behaviour = value;
            }
        }
        public MessageData(string messageText)
        {
            this.messageText = messageText;
        }
    }
}