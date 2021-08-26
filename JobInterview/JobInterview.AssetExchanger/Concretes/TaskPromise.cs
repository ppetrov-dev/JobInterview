﻿using System;
using System.Threading;
using System.Threading.Tasks;
using JobInterview.AssetExchanger.Abstractions;

namespace JobInterview.AssetExchanger.Concretes
{
    internal class TaskPromise<TResult> : ITaskPromise<TResult>
    {
        public TaskPromise(Task<TResult> task, CancellationToken cancellationToken)
        {
            task.ContinueWith(OnTaskCallback, cancellationToken);
        }

        public event Action<TResult>? Succeeded;

        private void OnTaskCallback(Task<TResult> task)
        {
            if (!task.IsCompletedSuccessfully)
                return;

            Succeeded?.Invoke(task.Result);
        }
    }
}