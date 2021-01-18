using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Models;

namespace VanillaWorks.MessageLogger.Runtime.Core.Extension
{
    public static class MessageDataExtension
    {
        public static  void DisplayMessage(this MessageData data)
        {
            MessageData.messagesController.DisplayMessage(data);
        }

        public static void DisposeMessage(this MessageData data)
        {
            MessageData.messagesController.DisposeMessage(data);
        }
    }
}
