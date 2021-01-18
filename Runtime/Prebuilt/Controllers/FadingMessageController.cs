using System;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Controllers;
using VanillaWorks.MessageLogger.Runtime.Prebuilt.Models;

namespace VanillaWorks.MessageLogger.Runtime.Prebuilt.Controllers
{
    public class FadingMessageController : ArchivedMessagesController<FadingMessage>
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                saveLoadService.Save((success)=>
                {
                    Debug.Log($"Messages Saved :{success}");
                });
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                saveLoadService.Load((success =>
                {
                    Debug.Log($"Messages Loaded :{success}");
                } ));
            }
        }
    }
}
