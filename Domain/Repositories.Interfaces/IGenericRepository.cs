using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Read
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        
        // Create
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        
        // Update
        void Update(T entity);
        
        // Delete
        void Remove(T entity);
        void SoftDelete(T entity);
    }
}