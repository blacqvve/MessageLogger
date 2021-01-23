using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Behaviours;
using Object = UnityEngine.Object;

namespace VanillaWorks.MessageLogger.Runtime.Core.Services
{
    public class PoolService:IDisposable
    {
        protected const int BackupPoolSize = 5;
        
        protected int prePoolSize;
        
        protected List<MessageBehaviour> pooledMessages;
        protected Type messageBehaviour;
        
        protected GameObject messagePrefab;
        protected Transform poolParent;
        protected Transform activeParent;

        public PoolService(int prePoolCount, Type messageBehaviour, GameObject messagePrefab,Transform activeParent)
        {
            prePoolSize = prePoolCount;
            this.messageBehaviour = messageBehaviour;
            this.messagePrefab = messagePrefab;
            this.activeParent = activeParent;
            pooledMessages = new List<MessageBehaviour>();

            poolParent = new GameObject("PoolParent").transform;
            PrePool(prePoolSize);
        }

        private void PrePool(int count)
        {
            if (count <= 0) return;

            for (int i = 0; i < count; i++)
            {
                GameObject gameObject =
                    Object.Instantiate(messagePrefab, poolParent);
                gameObject.SetActive(false);

                MessageBehaviour mb = gameObject.GetComponentInChildren<MessageBehaviour>(true);
                if (mb)
                {
                    mb.Initialize();
                    pooledMessages.Add(mb);
                }
                else
                {
                    mb = gameObject
                        .AddComponent(messageBehaviour) as MessageBehaviour;
                    if (!(mb is null))
                    {
                        mb.Initialize();
                        pooledMessages.Add(mb);
                    }
                    else
                    {
                        Debug.LogError("Message behaviour type incorrect");
                        Object.Destroy(gameObject);
                        return;
                    }
                }
            }
        }

        public MessageBehaviour GetFromPool()
        {
            if (pooledMessages.Count <= 0)
            {
                PrePool(BackupPoolSize);
            }
            MessageBehaviour mb = pooledMessages.FirstOrDefault(x => x.IsActive==false);
            if (mb is null)
            {
                PrePool(BackupPoolSize);
                mb = pooledMessages.FirstOrDefault(x => x.IsActive==false);
            }
            if (mb)
            {
                mb.transform.SetParent(activeParent);
                mb.gameObject.SetActive(true);
                return mb;
            }
            else
            {
                Debug.LogError("Something went wrong with pool service");
                return null;
            }
        }

        public void ReturnToPool(MessageBehaviour message)
        {
            message.transform.SetParent(poolParent);
            message.gameObject.SetActive(false);
            int activeCount = pooledMessages.Count(x => x.IsActive==false);
            if ( activeCount<= prePoolSize) return;
            
            pooledMessages.Remove(message);
            Object.Destroy(message.gameObject);
        }

        public void Dispose()
        {
            pooledMessages.Clear();
            Object.Destroy(poolParent);
#if UNITY_EDITOR
            Debug.Log($"Pool service disposed");
#endif
        }
    }
}