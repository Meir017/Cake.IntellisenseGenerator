﻿using Cake.Core;
using System;

namespace Cake.IntellisenseGenerator
{
    public abstract partial class CakeFile
    {
        protected ICakeContext Context => throw new NotSupportedException();

        public abstract void Execute();

        protected CakeTaskBuilder<ActionTask> Task(string name) => throw new NotSupportedException();

        protected void TaskSetup(Action<ICakeContext, ActionTask> action) => throw new NotSupportedException();
        protected void TaskTeardown(Action<ICakeContext, ActionTask> action) => throw new NotSupportedException();

        protected void Setup(Action<ICakeContext> action) => throw new NotSupportedException();
        protected void Teardown(Action<ICakeContext> action) => throw new NotSupportedException();

        protected void RunTarget(string name) => throw new NotSupportedException();

        protected void Addin(string addin) => throw new NotSupportedException();
        protected void Tool(string tool) => throw new NotSupportedException();
        protected void Load(string file) => throw new NotSupportedException();
    }
}
