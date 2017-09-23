using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTask
{
    public class ComplexCollection<TId, TName, TValue>
    {
        // ----------------------------------------------------
        // Вспомогательные словари

        // Основной словарь для хранения составного ключа и индексов значений в словарях _idDict и _nameDict
        private readonly Dictionary<KeyValuePair<TId, TName>, KeyValuePair<Int32, Int32>> _mainDict = new Dictionary<KeyValuePair<TId, TName>, KeyValuePair<Int32, Int32>>();
        
        // Словарь для быстрого получения значений по Id
        private readonly Dictionary<TId, IList<TValue>> _idDict = new Dictionary<TId, IList<TValue>>();
        
        // Словарь для быстрого получения значений по Name
        private readonly Dictionary<TName, IList<TValue>> _nameDict = new Dictionary<TName, IList<TValue>>();

        // ----------------------------------------------------
        // Объект для синхронизации потоков

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        // ----------------------------------------------------
        // Свойство - число элементов в коллекции

        public int Count
        {
            get { return _mainDict.Count; }
        }

        // ----------------------------------------------------
        // Добавление элемента в коллекцию

        public void Add(TId id, TName name, TValue value)
        {
            // Проверка существования ключа

            KeyValuePair<TId, TName> pairKey = new KeyValuePair<TId, TName>(id, name);
            if (_mainDict.ContainsKey(pairKey))
            {
                throw new ArgumentOutOfRangeException("Id, Name", "Элемент с таким ключом уже существует");
            }

            // Использование потокобезопасной конструкции

            _lock.EnterWriteLock();

            try
            {
                // Словарь значений для Id

                Int32 indexId;
                IList<TValue> valueList;
                if (_idDict.TryGetValue(id, out valueList))
                {
                    IList<TValue> listId = _idDict[id] as IList<TValue>;
                    listId.Add(value);
                    indexId = listId.Count - 1; // Индекс в _idDict для основного словаря
                }
                else
                {
                    _idDict.Add(id, new List<TValue>() { value });
                    indexId = 0; // Индекс в _idDict для основного словаря
                }

                // Словарь значений для Name

                Int32 indexName;
                if (_nameDict.TryGetValue(name, out valueList))
                {
                    IList<TValue> listName = _nameDict[name] as IList<TValue>;
                    listName.Add(value);
                    indexName = listName.Count - 1; // Индекс в _nameDict для основного словаря
                }
                else
                {
                    _nameDict.Add(name, new List<TValue>() { value });
                    indexName = 0; // Индекс в _nameDict для основного словаря
                }

                // Основной словарь

                KeyValuePair<Int32, Int32> pairIndexes = new KeyValuePair<Int32, Int32>(indexId, indexName);
                _mainDict.Add(pairKey, pairIndexes);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // ----------------------------------------------------
        // Удаление элемента коллекции

        public void Remove(TId id, TName name)
        {
            KeyValuePair<TId, TName> pairKey = new KeyValuePair<TId, TName>(id, name);

            if (_mainDict.ContainsKey(pairKey))
            {
                // Использование потокобезопасной конструкции

                _lock.EnterWriteLock();

                try
                {
                    // Удаление элемента из словаря для Id

                    Int32 indexID = _mainDict[pairKey].Key;
                    IList<TValue> listId = _idDict[pairKey.Key] as IList<TValue>;
                    listId.RemoveAt(indexID);
                    if (listId.Count == 0) _idDict.Remove(pairKey.Key);

                    // Удаление элемента из словаря для Name

                    Int32 indexName = _mainDict[pairKey].Value;
                    IList<TValue> listName = _nameDict[pairKey.Value] as IList<TValue>;
                    listName.RemoveAt(indexName);
                    if (listName.Count == 0) _nameDict.Remove(pairKey.Value);

                    // Удаление элемента из основного словаря

                    _mainDict.Remove(pairKey);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Id, Name", "Элемента с таким ключом не существует");
            }
        }

        // ----------------------------------------------------
        // Получение элементов по Id

        public IList<TValue> GetValuesById(TId id)
        {
            _lock.EnterReadLock();

            try
            {
                IList<TValue> valueList;
                if (_idDict.TryGetValue(id, out valueList))
                {
                    return valueList;
                }
                else
                {
                    return new List<TValue>();
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        // ----------------------------------------------------
        // Получение элементов по Name

        public IList<TValue> GetValuesByName(TName name)
        {
            _lock.EnterReadLock();

            try
            {

                IList<TValue> valueList;
                if (_nameDict.TryGetValue(name, out valueList))
                {
                    return valueList;
                }
                else
                {
                    return new List<TValue>();
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        // ----------------------------------------------------
        // Получение элемента по составному ключу

        public TValue GetValue(TId id, TName name)
        {
            _lock.EnterReadLock();

            try
            {
                KeyValuePair<TId, TName> pairKey = new KeyValuePair<TId, TName>(id, name);
                if (_mainDict.ContainsKey(pairKey))
                {
                    Int32 indexID = _mainDict[pairKey].Key;
                    IList<TValue> listId = _idDict[pairKey.Key] as IList<TValue>;
                    return listId[indexID];
                }
                else
                {
                    return default(TValue);
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        // ----------------------------------------------------
        // Строковое представление коллекции

        public override string ToString()
        {
            _lock.EnterReadLock();

            try
            {
                StringBuilder sb = new StringBuilder();

                // Коллекция со значениями

                foreach (var pair in _mainDict)
                {
                    KeyValuePair<TId, TName> pairKey = (KeyValuePair<TId, TName>)pair.Key;
                    KeyValuePair<Int32, Int32> pairIndexes = (KeyValuePair<Int32, Int32>)pair.Value;
                    sb.AppendLine(pair.Key.ToString() + " " + _idDict[pairKey.Key][pairIndexes.Key]);
                }

                // Вспомогательные словари (для отладки)

                /*
                sb.AppendLine();
                sb.AppendLine("Словарь значений Id:");
                foreach (var p1 in _idDict)
                {
                    sb.Append(p1.Key.ToString() + ":");
                    IList<TValue> listId = _idDict[p1.Key] as IList<TValue>;
                    foreach (var pp1 in listId)
                    {
                        sb.Append(" " + pp1);
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine("Словарь значений Name:");
                foreach (var p2 in _nameDict)
                {
                    sb.Append(p2.Key.ToString() + ":");
                    IList<TValue> listName = _nameDict[p2.Key] as IList<TValue>;
                    foreach (var pp2 in listName)
                    {
                        sb.Append(" " + pp2);
                    }
                    sb.AppendLine();
                }
                */

                return sb.ToString();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
