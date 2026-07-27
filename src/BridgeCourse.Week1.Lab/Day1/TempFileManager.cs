using System;
using System.IO;

namespace BridgeCourse.Week1.Lab.Day1
{
    /// <summary>
    /// TempFileManager implements the complete and robust IDisposable pattern.
    /// It creates a temporary file in the constructor and deletes it when disposed or finalized.
    /// </summary>
    public class TempFileManager : IDisposable
    {
        private bool _disposed = false;

        public string FilePath { get; }

        public TempFileManager()
        {
            // Create a temporary file and get its path
            FilePath = Path.GetTempFileName();
            
            // Write some initial text into the temporary file
            File.WriteAllText(FilePath, "Temporary file content created by TempFileManager.");
            Console.WriteLine($"[TempFileManager] Temp file created at: {FilePath}");
        }

        /// <summary>
        /// Reads the content of the temporary file.
        /// Throws ObjectDisposedException if the manager has already been disposed.
        /// </summary>
        public string ReadContent()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(TempFileManager), "Cannot read content: TempFileManager has been disposed.");
            }

            return File.ReadAllText(FilePath);
        }

        /// <summary>
        /// Public implementation of Dispose, called by client code.
        /// </summary>
        public void Dispose()
        {
            // Call Dispose(true) to clean up both managed and unmanaged resources
            Dispose(true);
            
            // GC.SuppressFinalize tells the Garbage Collector that this object has already been cleaned up.
            // This prevents the GC from calling the finalizer (~TempFileManager), saving resource overhead
            // and avoiding execution of cleanup code twice.
            GC.SuppressFinalize(this);
            
            Console.WriteLine("[TempFileManager] Dispose() called. Suppressed finalization.");
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">
        /// True if called from client Dispose() code (can safely dispose managed objects).
        /// False if called from the Finalizer (cannot trust managed references as they might have been garbage collected already).
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clean up managed resources here if any exist.
                    // For example: components.Dispose(), streamReader.Dispose(), etc.
                    Console.WriteLine("[TempFileManager] Disposing managed resources.");
                }

                // Clean up unmanaged/external resources here (e.g., delete the physical file).
                // Note: File deletion is an OS operation, so it must run in both disposing paths.
                if (File.Exists(FilePath))
                {
                    try
                    {
                        File.Delete(FilePath);
                        Console.WriteLine($"[TempFileManager] Temp file deleted successfully from: {FilePath}");
                    }
                    catch (Exception ex)
                    {
                        // In a finalizer, we must NEVER let exceptions propagate out of the finalizer thread,
                        // as it can crash the entire application process.
                        Console.WriteLine($"[TempFileManager] Warning: Failed to delete temp file during disposal: {ex.Message}");
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizer (safety net). 
        /// This is called by the Garbage Collector if the client forgot to call Dispose().
        /// </summary>
        ~TempFileManager()
        {
            Console.WriteLine("[TempFileManager] Finalizer (~TempFileManager) called! Cleaning up unmanaged/external resources.");
            
            // Call Dispose(false) because managed resources might have already been collected
            // by the Garbage Collector. We only clean up unmanaged/external resources (like the file) here.
            Dispose(false);
        }
    }
}
