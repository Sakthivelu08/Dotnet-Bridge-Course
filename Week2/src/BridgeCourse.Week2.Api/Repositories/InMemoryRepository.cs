using System;
using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week2.Api.Models;

namespace BridgeCourse.Week2.Api.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _data = new List<T>();
        private readonly object _lock = new object();
        private int _nextId = 1;

        public IEnumerable<T> GetAll()
        {
            lock (_lock)
            {
                return _data.ToList();
            }
        }

        public T? GetById(int id)
        {
            lock (_lock)
            {
                return _data.FirstOrDefault(item => item.Id == id);
            }
        }

        public void Add(T entity)
        {
            lock (_lock)
            {
                if (entity.Id <= 0)
                {
                    entity.Id = _nextId++;
                }
                else
                {
                    if (_data.Any(item => item.Id == entity.Id))
                    {
                        throw new InvalidOperationException($"An item with ID {entity.Id} already exists.");
                    }
                    if (entity.Id >= _nextId)
                    {
                        _nextId = entity.Id + 1;
                    }
                }
                _data.Add(entity);
            }
        }

        public void Update(T entity)
        {
            lock (_lock)
            {
                var index = _data.FindIndex(item => item.Id == entity.Id);
                if (index < 0)
                {
                    throw new KeyNotFoundException($"Item with ID {entity.Id} was not found.");
                }
                _data[index] = entity;
            }
        }

        public void Delete(int id)
        {
            lock (_lock)
            {
                var item = _data.FirstOrDefault(i => i.Id == id);
                if (item != null)
                {
                    _data.Remove(item);
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _data.Clear();
                _nextId = 1;
            }
        }
    }
}
