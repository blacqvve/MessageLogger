using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Models;

namespace VanillaWorks.MessageLogger.Runtime.Core.Services
{
    [Serializable]
    public class ArchiveService<T> where T : MessageData
    {
        protected int bufferCapacity;

        protected List<T> messageBuffer;
        public List<T> MessageBuffer => messageBuffer;

        public ArchiveService()
        {
            messageBuffer = new List<T>();
        }

        public ArchiveService(int bufferCapacity) : this()
        {
            this.bufferCapacity = bufferCapacity;
            messageBuffer.Capacity = this.bufferCapacity > messageBuffer.Count
                ? this.bufferCapacity
                : messageBuffer.Capacity;
        }

        public void AddToArchive(T data)
        {
            if (data is null) return;
            messageBuffer.Add(data);
        }

        public void AddToArchive(IEnumerable<T> dataList)
        {
            if (dataList is null)
                return;

            foreach (var data in dataList)
            {
                messageBuffer.Add(data);
            }
        }

        public void LoadBuffer(object obj)
        {
            if (!(obj is SerializedData saveData))
            {
                return;
            }

            messageBuffer = new List<T>(saveData.data);

#if UNITY_EDITOR
            Debug.Log($"Buffer loaded \n DataCount = {messageBuffer.Count}");
#endif
        }

        public object GetSerializableData()
        {
            return new SerializedData(messageBuffer);
        }

        [Serializable]
        public class SerializedData
        {
            public List<T> data;

            public SerializedData(List<T> data)
            {
                this.data = data;
            }
        }
    }
}