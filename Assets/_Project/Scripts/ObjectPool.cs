using System.Collections.Generic;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; set; }
        private readonly Dictionary<string, Queue<GameObject>> _objectPool = new();

        public GameObject GetObject(GameObject requestGameObject)
        {
            if (!_objectPool.TryGetValue(requestGameObject.name, out Queue<GameObject> objectList))
                return CreateNewObject(requestGameObject);
            if (objectList.Count == 0)
                return CreateNewObject(requestGameObject);

            var activeObject = objectList.Dequeue();
            activeObject.SetActive(true);
            return activeObject;
        }
    
        private static GameObject CreateNewObject(GameObject gameObject)
        {
            var newGameObject = Instantiate(gameObject);
            newGameObject.name = gameObject.name;
            return newGameObject;
        }
    
        public void ReturnGameObject(GameObject requestGameObject)
        {
            if (_objectPool.TryGetValue(requestGameObject.name, out var objectList))
                objectList.Enqueue(requestGameObject);
            else
            {
                var newObjectQueue = new Queue<GameObject>();
                newObjectQueue.Enqueue(requestGameObject);
                _objectPool.Add(requestGameObject.name, newObjectQueue);
            }
        
            requestGameObject.SetActive(false);
        }
    }
}