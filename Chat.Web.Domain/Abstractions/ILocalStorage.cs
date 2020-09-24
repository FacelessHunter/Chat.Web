using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Domain.Abstractions
{
    public interface ILocalStorageService
    {
        /// <summary>
        /// Clears all data from local storage.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
        ValueTask ClearAsync();

        /// <summary>
        /// Retrieve the specified data from local storage and deseralise it to the specfied type.
        /// </summary>
        /// <param name="key">A <see cref="string"/> value specifying the name of the local storage slot to use</param>
        /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
        ValueTask<T> GetItemAsync<T>(string key);


        /// <summary>
        /// Checks if the <paramref name="key"/> exists in local storage, but does not check its value.
        /// </summary>
        /// <param name="key">A <see cref="string"/> value specifying the name of the storage slot to use</param>
        /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
        ValueTask<bool> ContainKeyAsync(string key);

        /// <summary>
        /// Remove the data with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">A <see cref="string"/> value specifying the name of the storage slot to use</param>
        /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
        ValueTask RemoveItemAsync(string key);

        /// <summary>
        /// Sets or updates the <paramref name="data"/> in local storage with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">A <see cref="string"/> value specifying the name of the storage slot to use</param>
        /// <param name="data">The data to be saved</param>
        /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
        ValueTask SetItemAsync<T>(string key, T data);
    }
}
