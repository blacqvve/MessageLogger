using UnityEngine;
using VanillaWorks.MessageLogger.Runtime.Attributes;
using VanillaWorks.MessageLogger.Runtime.Core.Behaviours;
using VanillaWorks.MessageLogger.Runtime.Core.Models;
using VanillaWorks.MessageLogger.Runtime.Core.Services;

namespace VanillaWorks.MessageLogger.Runtime.Core.Controllers
{
    public class ArchivedMessagesController<T> : MessagesControllerBase where T  : MessageData
    {
        [Space] [Header("Archive Service Fields")] [SerializeField]
        protected int _bufferCapacity = 300;

        [Space] [Header("Save Load Service Fields")] [SerializeField]
        private bool _useSaveSystem;

        [Space] [SerializeField] [ConditionalHide("_useSaveSystem", true)]
        private bool _autoSave;

        [SerializeField,Range(15f,3600f)]
        [ConditionalHide(new[] {"_autoSave", "_useSaveSystem"}, true, false)]
        [Tooltip("Time between auto saves in seconds")]
        protected float autoSaveInterval=30f;

        protected ArchiveService<T> archiveService;
        protected SaveLoadService<T> saveLoadService;


        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
        protected override void Activate(MessageData data, MessageBehaviour behaviour)
        {
            archiveService.AddToArchive(data as T);
            base.Activate(data, behaviour);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_useSaveSystem)
                saveLoadService.Dispose();
        }
        private void Initialize()
        {
            archiveService = new ArchiveService<T>(_bufferCapacity);

            if (_useSaveSystem)
            {
                saveLoadService =
                    new SaveLoadService<T>(archiveService, this, _autoSave, autoSaveInterval);
                saveLoadService.Load();
            }
        }
    }
}