using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reservations.Domain.ReservationAggregate;
using Reservations.Domain.Shared;
using Reservations.Domain.StatusAggregate;
using Reservations.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Infrastructure
{
    public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        private readonly List<ApplicationDbContext> _applicationDbContexts = new();

        private IReservationRepository _reservationRepository;
        private IStatusRepository _statusRepository;

        private bool _createContext;
        private bool _disposed;

        public UnitOfWork(ApplicationDbContext applicationDbContext, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _context = applicationDbContext;
        }

        public IReservationRepository ReservationRepository
        {
            get
            {
                if (_createContext)
                {
                    _createContext = false;

                    return new ReservationRepository(CreateDbContext());
                }

                return _reservationRepository ??= new ReservationRepository(_context);
            }
        }

        public IStatusRepository StatusRepository
        {
            get
            {
                if (_createContext)
                {
                    _createContext = false;

                    return new StatusRepository(CreateDbContext());
                }

                return _statusRepository ??= new StatusRepository(_context);
            }
        }

        public IUnitOfWork CreateContext()
        {
            _createContext = true;
            return this;
        }

        private ApplicationDbContext CreateDbContext()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            var dbContext = new ApplicationDbContext(options);
            _applicationDbContexts.Add(dbContext);
            return dbContext;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            for (var i = 0; i < _applicationDbContexts.Count; i++)
            {
                if (_applicationDbContexts[i] != null)
                {
                    _applicationDbContexts[i].Dispose();
                    _applicationDbContexts[i] = null;
                }
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _context?.Dispose();
            }

            _disposed = true;
        }

        private async ValueTask DisposeAsyncCore()
        {
            for (var i = 0; i < _applicationDbContexts.Count; i++)
            {
                if (_applicationDbContexts[i] != null)
                {
                    await _applicationDbContexts[i].DisposeAsync().ConfigureAwait(false);

                    _applicationDbContexts[i] = null;
                }
            }
        }
    }
}
