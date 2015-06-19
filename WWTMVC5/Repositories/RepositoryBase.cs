//-----------------------------------------------------------------------
// <copyright file="RepositoryBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Abstract base repository class having the implementation for methods which are
    /// common to all the db entities.
    /// </summary>
    /// <typeparam name="T">Template object</typeparam>
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        /// <summary>
        /// Layerscape database context
        /// </summary>
        private EarthOnlineEntities _earthOnlineDbContext;

        /// <summary>
        /// Initializes a new instance of the RepositoryBase class.
        /// </summary>
        /// <param name="earthOnlineDbContext">Database context of Layerscape DB</param>
        public RepositoryBase(EarthOnlineEntities earthOnlineDbContext)
        {
            _earthOnlineDbContext = earthOnlineDbContext;
        }

        /// <summary>
        /// Gets or sets the Layerscape database context
        /// </summary>
        public EarthOnlineEntities EarthOnlineDbContext
        {
            get
            {
                return _earthOnlineDbContext;
            }
            set
            {
                _earthOnlineDbContext = value;
            }
        }

        /// <summary>
        /// Gets the DbSet instance from the Layerscape database context
        /// </summary>
        protected DbSet<T> DbSet
        {
            get
            {
                return EarthOnlineDbContext.Set<T>();
            }
        }

        /// <summary>
        /// Adds the given entity to the Layerscape database.
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        public async void AddAsync(T entity)
        {
            await DbSet.LoadAsync();
            DbSet.Add(entity);
        }
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        /// <summary>
        /// Updates the given Entity in the Layerscape database.
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        public void Update(T entity)
        {
            DbSet.Attach(entity);
            EarthOnlineDbContext.Entry(entity).State = EntityState.Modified;
        }

        public async void UpdateAsync(T entity)
        {
            await DbSet.LoadAsync();
            DbSet.Attach(entity);
            EarthOnlineDbContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes the given entity from the Layerscape database.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public async void DeleteAsync(T entity)
        {
            await DbSet.LoadAsync();
            DbSet.Remove(entity);
        }
        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        /// <summary>
        /// Gets the entity from the DbSet for the given object template which is having the given id.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <returns>Entity with the given id</returns>
        public async Task<T> GetByIdAsync(Guid id)
        {
            await DbSet.LoadAsync();
            return DbSet.Find(id);
        }
        public T GetById(Guid id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// Gets the Entity which satisfies the given condition from the collection of entities.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <returns>Entity which satisfies the condition</returns>
        public async Task<T> GetItemAsync(Expression<Func<T, bool>> condition)
        {
            await DbSet.LoadAsync();
            return DbSet.Where(condition).FirstOrDefault();
        }

        public T GetItem(Expression<Func<T, bool>> condition)
        {
            return DbSet.Where(condition).FirstOrDefault();
        }

        /// <summary>
        /// Gets the Entity which satisfies the given condition from the collection of entities and also load the navigation property
        /// mentioned in Include.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <param name="include">Navigation property to be included</param>
        /// <returns>Entity which satisfies the condition</returns>
        public async Task<T> GetItemAsync(Expression<Func<T, bool>> condition, string include)
        {
            await DbSet.LoadAsync();
            return DbSet.Where(condition).Include(include).FirstOrDefault();
        }
        public T GetItem(Expression<Func<T, bool>> condition, string include)
        {
            return DbSet.Where(condition).Include(include).FirstOrDefault();
        }

        /// <summary>
        /// Gets the items count satisfying the given condition in the current database table.
        /// </summary>
        /// <param name="condition">Condition to be applied</param>
        /// <returns>Count of items satisfying the condition</returns>
        public async Task<int> GetItemsCountAsync(Expression<Func<T, bool>> condition)
        {
            await DbSet.LoadAsync();
            return DbSet.Count(condition);
        }
        public int GetItemsCount(Expression<Func<T, bool>> condition)
        {
            return DbSet.Count(condition);
        }

        /// <summary>
        /// Gets multiple Entities which satisfies the given condition from the collection of entities.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <param name="orderBy">Order by clause</param>
        /// <param name="descending">Order by descending?</param>
        /// <returns>Collection of Entities</returns>
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending)
        {
            IEnumerable<T> result;
            await DbSet.LoadAsync();
            if (orderBy == null && condition == null)
            {
                // When condition and order by are not passed, return all the items.
                result = DbSet.ToList();
            }
            else if (orderBy != null && condition == null)
            {
                // When condition is not passed, only order by is passed, return data for given order by.
                result = @descending ? DbSet.OrderByDescending(orderBy).ToList() : DbSet.OrderBy(orderBy).ToList();
            }
            else if (orderBy == null)
            {
                // When order by is not passed, only condition is passed, return data for given condition.
                result = DbSet.Where(condition).ToList();
            }
            else
            {
                // When both order by and condition are passed, return data for given condition and order by.
                result = @descending ? DbSet.Where(condition).OrderByDescending(orderBy).ToList() : DbSet.Where(condition).OrderBy(orderBy).ToList();
            }

            return result;
        }
        public IEnumerable<T> GetItems(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending)
        {
            IEnumerable<T> result;
            
            if (orderBy == null && condition == null)
            {
                // When condition and order by are not passed, return all the items.
                result = DbSet.ToList();
            }
            else if (orderBy != null && condition == null)
            {
                // When condition is not passed, only order by is passed, return data for given order by.
                result = @descending ? DbSet.OrderByDescending(orderBy).ToList() : DbSet.OrderBy(orderBy).ToList();
            }
            else if (orderBy == null)
            {
                // When order by is not passed, only condition is passed, return data for given condition.
                result = DbSet.Where(condition).ToList();
            }
            else
            {
                // When both order by and condition are passed, return data for given condition and order by.
                result = @descending ? DbSet.Where(condition).OrderByDescending(orderBy).ToList() : DbSet.Where(condition).OrderBy(orderBy).ToList();
            }

            return result;
        }

        /// <summary>
        /// Gets multiple Entities which satisfies the given condition from the collection of entities.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <param name="orderBy">Order by clause</param>
        /// <param name="descending">Order by descending?</param>
        /// <param name="skipCount">Number of items to be skipped.</param>
        /// <param name="takeCount">Number of items to be picked up.</param>
        /// <returns>Collection of Entities</returns>
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending, int skipCount, int takeCount)
        {
            IEnumerable<T> result;
            await DbSet.LoadAsync();
            if (orderBy == null && condition == null)
            {
                // When condition and order by are not passed, return all the items.
                result = DbSet
                    .Skip(skipCount)
                    .Take(takeCount)
                    .ToList();
            }
            else if (orderBy != null && condition == null)
            {
                // When condition is not passed, only order by is passed, return data for given order by.
                if (descending)
                {
                    result = DbSet
                        .OrderByDescending(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
                else
                {
                    result = DbSet
                        .OrderBy(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
            }
            else if (orderBy == null)
            {
                // When order by is not passed, only condition is passed, return data for given condition.
                result = DbSet
                    .Where(condition)
                    .Take(takeCount)
                    .ToList();
            }
            else
            {
                // When both order by and condition are passed, return data for given condition and order by.
                if (descending)
                {
                    result = DbSet.Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
                else
                {
                    result = DbSet
                        .Where(condition).OrderBy(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
            }

            return result;
        }
        public IEnumerable<T> GetItems(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending, int skipCount, int takeCount)
        {
            IEnumerable<T> result;
            
            if (orderBy == null && condition == null)
            {
                // When condition and order by are not passed, return all the items.
                result = DbSet
                    .Skip(skipCount)
                    .Take(takeCount)
                    .ToList();
            }
            else if (orderBy != null && condition == null)
            {
                // When condition is not passed, only order by is passed, return data for given order by.
                if (descending)
                {
                    result = DbSet
                        .OrderByDescending(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
                else
                {
                    result = DbSet
                        .OrderBy(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
            }
            else if (orderBy == null)
            {
                // When order by is not passed, only condition is passed, return data for given condition.
                result = DbSet
                    .Where(condition)
                    .Take(takeCount)
                    .ToList();
            }
            else
            {
                // When both order by and condition are passed, return data for given condition and order by.
                if (descending)
                {
                    result = DbSet.Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
                else
                {
                    result = DbSet
                        .Where(condition).OrderBy(orderBy)
                        .Skip(skipCount)
                        .Take(takeCount)
                        .ToList();
                }
            }

            return result;
        }
       

        /// <summary>
        /// Gets all the Entities from the entities collection.
        /// </summary>
        /// <param name="orderBy">Order by clause</param>
        /// <returns>Collection of Entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync(Func<T, object> orderBy)
        {
            await DbSet.LoadAsync();
            if (orderBy == null)
            {
                return DbSet.ToList();
            }
            return DbSet.OrderBy(orderBy).ToList();
        }
        public IEnumerable<T> GetAll(Func<T, object> orderBy)
        {   
            if (orderBy == null)
            {
                return DbSet.ToList();
            }
            return DbSet.OrderBy(orderBy).ToList();
        }

        /// <summary>
        /// Saves the changes made in the data models to the Layerscape database.
        /// </summary>
        public async void SaveChangesAsync()
        {
            await EarthOnlineDbContext.SaveChangesAsync();

        }
        public void SaveChanges()
        {
            EarthOnlineDbContext.SaveChanges();
        }
    }
}