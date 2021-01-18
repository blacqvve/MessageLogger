using System;
using System.Linq;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Controllers;
using VanillaWorks.MessageLogger.Runtime.Core.Extension;
using VanillaWorks.MessageLogger.Runtime.Core.Models;
using VanillaWorks.MessageLogger.Runtime.Prebuilt;
using VanillaWorks.MessageLogger.Runtime.Prebuilt.Models;
using Random = UnityEngine.Random;

namespace VanillaWorks.MessageLogger.Runtime
{
    public class Test : MonoBehaviour
    {
        private MessageData _messageData;

        private void Start()
        {
            _messageData = new FadingMessage(RandomString(Random.Range(4, 12)),
                Random.value >= 0.5f ? RandomColor() : Color.white);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                new FadingMessage(RandomString(Random.Range(4, 12)),
                    Random.value >= 0.5f ? RandomColor() : Color.white).DisplayMessage();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _messageData.DisplayMessage();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _messageData.DisposeMessage();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                GenerateMessage();
            }
        }

        private void GenerateMessage()
        {
            for (int i = 0; i < 1000; i++)
            {
                var messageData = new FadingMessage(RandomString(Random.Range(4, 12)),
                    Random.value >= 0.5f ? RandomColor() : Color.white);
                messageData.DisplayMessage();
            }
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Range(0, s.Length)]).ToArray());
        }

        public Color RandomColor()
        {
            return new Color(Random.value, Random.value, Random.value);
        }
    }
}