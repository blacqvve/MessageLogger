using System;
using System.Collections;
using System.IO;
using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Core.Controllers;
using VanillaWorks.MessageLogger.Runtime.Core.Models;

namespace VanillaWorks.MessageLogger.Runtime.Core.Services
{
    [Serializable]
    public class SaveLoadService<T> : IDisposable where T : MessageData
    {
        protected const string Filename = nameof(T) + ".json";
        protected ArchiveService<T> archiveService;
        protected MessagesControllerBase messagesController;
        protected float saveInterval;
        protected Coroutine autoSaveRoutine;
        protected bool autoSaveEnabled;
        protected string path;

        public SaveLoadService(ArchiveService<T> archiveService,
            MessagesControllerBase messagesController)
        {
            if (archiveService == null)
            {
                Debug.LogError("Archive Service is null, save load service cant work without it");
                return;
            }
            Application.quitting += OnApplicationQuitting;
            path = Application.persistentDataPath + @"\" + Filename;
            this.archiveService = archiveService;
            this.messagesController = messagesController;
        }

        public SaveLoadService(ArchiveService<T> archiveService,
            MessagesControllerBase messagesController, bool autoSave, float saveInterval) : this(archiveService,
            messagesController)
        {
            this.archiveService = archiveService;
            this.saveInterval = saveInterval;
            this.messagesController = messagesController;
            autoSaveEnabled = autoSave;

            if (autoSaveEnabled)
            {
                StartAutoSaveRoutine();
            }
        }

        public virtual void Save(Action<bool> completeCallBack = null)
        {
            messagesController.StartCoroutine(SaveRoutine(completeCallBack));
        }

        public virtual void Load(Action<bool> completeCallBack = null)
        {
            messagesController.StartCoroutine(LoadRoutine(completeCallBack));
        }

        public virtual void EnableAutoSave(float? interval = null)
        {
            if (interval.HasValue)
            {
                saveInterval = interval.Value;
            }

            StartAutoSaveRoutine();
        }

        public virtual void DisableAutoSave()
        {
            this.Save();
            messagesController.StopCoroutine(autoSaveRoutine);
        }

        public virtual void Dispose()
        {
#if UNITY_EDITOR
            Debug.Log("Save load system disposed");
#endif
            Application.quitting -= OnApplicationQuitting;
        }
        protected virtual void OnApplicationQuitting()
        {
            Application.quitting -= OnApplicationQuitting;
            SaveOnMainThread();
        }

        protected virtual void SaveOnMainThread(Action<bool> completeCallBack = null)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using (StreamWriter writer = new StreamWriter(fs))
            {
                string json = JsonUtility.ToJson(archiveService.GetSerializableData());
                var writeTask = writer.WriteAsync(json);
                writeTask.Wait();
#if UNITY_EDITOR
                Debug.Log("Main thread save finished");
#endif
            }
            fs.Close();
            completeCallBack?.Invoke(true);
        }

        private void StartAutoSaveRoutine()
        {
            if (autoSaveRoutine != null)
            {
                messagesController.StopCoroutine(autoSaveRoutine);
            }

            autoSaveRoutine = messagesController.StartCoroutine(AutoSaveRoutine());
        }

        private IEnumerator SaveRoutine(Action<bool> completeCallback)
        {
#if UNITY_EDITOR
            Debug.Log("Save started");
#endif
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using (StreamWriter writer = new StreamWriter(fs))
            {
                string json = JsonUtility.ToJson(archiveService.GetSerializableData());
                var writeTask = writer.WriteAsync(json);
#if UNITY_EDITOR
                float elapsedTime = 0f;
                while (!writeTask.IsCompleted)
                {
                    elapsedTime += Time.deltaTime;
                    Debug.Log("Save operation, Elapsed time " + elapsedTime.ToString("F4"));
                }
#endif
                yield return new WaitWhile(() => !writeTask.IsCompleted);
                Debug.Log("Finished");
            }
#if UNITY_EDITOR
            Debug.Log("Save finished");
#endif
            fs.Close();
            if (autoSaveEnabled)
            {
                StartAutoSaveRoutine();
            }

            completeCallback?.Invoke(true);
        }

        private IEnumerator LoadRoutine(Action<bool> completeCallback)
        {
#if UNITY_EDITOR
            Debug.Log("Load started");
#endif
            if (!File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.Log("There is no save file, returning from load method");
#endif
                yield break;
            }

            string json;

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            using (StreamReader reader = new StreamReader(fs))
            {
                var readTask = reader.ReadToEndAsync();
#if UNITY_EDITOR
                float elapsedTime = 0f;
                while (!readTask.IsCompleted)
                {
                    elapsedTime += Time.deltaTime;
                    Debug.Log("Reading Data.Elapsed Time :" + elapsedTime.ToString("F4"));
                }
#endif
                yield return new WaitWhile(() => !readTask.IsCompleted);
                json = readTask.Result;
            }
            fs.Close();
            var data = JsonUtility.FromJson<ArchiveService<T>.SerializedData>(json);
            if (data == null)
            {
#if UNITY_EDITOR
                Debug.Log("Save data is empty, not replacing with old buffer");
#endif
                yield break;
            }
            archiveService.LoadBuffer(data);
            completeCallback?.Invoke(true);
#if UNITY_EDITOR
            Debug.Log("Load finished");
#endif
        }

        private IEnumerator AutoSaveRoutine()
        {
            if (saveInterval <= 0f)
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"Save interval is too short, this can cause problems." +
                    $" Please call {nameof(SaveLoadService<T>)} service EnableAutoSave() method ");
#endif
                yield break;
            }
#if UNITY_EDITOR
            Debug.Log("Auto save routine started");
#endif
            yield return new WaitForSeconds(saveInterval);
            this.Save();
        }


    }
}