namespace ILSpy
{
    class Lock
    {
        public static Lock Create() => new Lock();

        readonly object lockObj;
        int recurseCount;
        Lock()
        {
            lockObj = new object();
            recurseCount = 0;
        }

        public void EnterReadLock()
        {
            Monitor.Enter(lockObj);
            if (recurseCount != 0)
            {
                Monitor.Exit(lockObj);
                throw new Exception("Recursive locks aren't supported");
            }
            recurseCount++;
        }

        public void ExitReadLock()
        {
            if (recurseCount <= 0)
                throw new Exception("Too many exit lock method calls");
            recurseCount--;
            Monitor.Exit(lockObj);
        }

        public void EnterWriteLock()
        {
            Monitor.Enter(lockObj);
            if (recurseCount != 0)
            {
                Monitor.Exit(lockObj);
                throw new Exception("Recursive locks aren't supported");
            }
            recurseCount--;
        }

        public void ExitWriteLock()
        {
            if (recurseCount >= 0)
                throw new Exception("Too many exit lock method calls");
            recurseCount++;
            Monitor.Exit(lockObj);
        }

    }
}