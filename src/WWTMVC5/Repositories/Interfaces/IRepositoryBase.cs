//-----------------------------------------------------------------------
// <copyright file="IRepositoryBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Base repository interface stating the methods to be implemented by the domain
    /// object repositories.
    /// </summary>
    /// <typeparam name="T">Entity Type for which is repository to be implemented</typeparam>
    public interface IRepositoryBase<T> where T : class
    {
        /// <summary>
        /// Adds the given entity to the Layerscape database.
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        void Add(T entity);

        /// <summary>
        /// Updates the given Entity in the Layerscape database.
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        void Update(T entity);

        /// <summary>
        /// Deletes the given entity from the Layerscape database.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(T entity);

        /// <summary>
        /// Gets the Entity which satisfies the given condition from the collection of entities.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <returns>Entity which satisfies the condition</returns>
        Task<T> GetItemAsync(Expression<Func<T, bool>> condition);
        T GetItem(Expression<Func<T, bool>> condition);

        /// <summary>
        /// Gets the Entity which satisfies the given condition from the collection of entities and also load the navigation property
        /// mentioned in Include.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <param name="include">Navigation property to be included</param>
        /// <returns>Entity which satisfies the condition</returns>
        Task<T> GetItemAsync(Expression<Func<T, bool>> condition, string include);
        T GetItem(Expression<Func<T, bool>> condition, string include);

        /// <summary>
        /// Gets the items count satisfying the given condition in the current database table.
        /// </summary>
        /// <param name="condition">Condition to be applied</param>
        /// <returns>Count of items satisfying the condition</returns>
        Task<int> GetItemsCountAsync(Expression<Func<T, bool>> condition);
        int GetItemsCount(Expression<Func<T, bool>> condition);

        /// <summary>
        /// Gets multiple Entities which satisfies the given condition from the collection of entities.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <param name="orderBy">Order by clause</param>
        /// <param name="descending">Order by descending?</param>
        /// <returns>Collection of Entities</returns>
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending);
        IEnumerable<T> GetItems(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending);

        /// <summary>
        /// Gets multiple Entities which satisfies the given condition from the collection of entities.
        /// </summary>
        /// <param name="condition">Condition to be satisfied</param>
        /// <param name="orderBy">Order by clause</param>
        /// <param name="descending">Order by descending?</param>
        /// <param name="skipCount">Number of items to be skipped.</param>
        /// <param name="takeCount">Number of items to be picked up.</param>
        /// <returns>Collection of Entities</returns>
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending, int skipCount, int takeCount);
        IEnumerable<T> GetItems(Expression<Func<T, bool>> condition, Func<T, object> orderBy, bool descending, int skipCount, int takeCount);

        /// <summary>
        /// Gets all the Entities from the entities collection.
        /// </summary>
        /// <param name="orderBy">Order by clause</param>
        /// <returns>Collection of Entities</returns>
        Task<IEnumerable<T>> GetAllAsync(Func<T, object> orderBy);
        IEnumerable<T> GetAll(Func<T, object> orderBy);

        /// <summary>
        /// Saves the changes made in the data models to the Layerscape database.
        /// </summary>
        void SaveChanges();
        void SaveChangesAsync();
    }
}