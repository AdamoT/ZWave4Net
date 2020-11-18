using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZWave.Utils
{
    internal class AsyncLazyProperty<T>
    {
        public delegate Task<T> ValueFactory(T existingValue, CancellationToken cancellationToken);

        private readonly ValueFactory _ValueFactory = null;

        private Task<T> _ValueFactoryTask = null;

        public AsyncLazyProperty(ValueFactory valueFactory)
        {
            _ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public async Task<T> GetValue(CancellationToken cancellationToken = default)
        {
            if (_ValueFactoryTask == null)
                lock (this)
                {
                    if (_ValueFactoryTask == null) _ValueFactoryTask = _ValueFactory.Invoke(default, cancellationToken);
                }

            return await _ValueFactoryTask
                .ConfigureAwait(false);
        }

        public async Task<T> RefreshValue(CancellationToken cancellationToken = default)
        {
            if (_ValueFactoryTask == null)
                return await GetValue(cancellationToken)
                    .ConfigureAwait(false);
            try
            {
                await _ValueFactoryTask
                    .ConfigureAwait(false);
            }
            catch
            {
            }

            lock (this)
            {
                if (_ValueFactoryTask.IsCompleted)
                {
                    var currentValue = _ValueFactoryTask.IsFaulted ? default : _ValueFactoryTask.Result;
                    _ValueFactoryTask = _ValueFactory.Invoke(currentValue, cancellationToken);
                }
            }

            return await _ValueFactoryTask
                .ConfigureAwait(false);
        }
    }
}
