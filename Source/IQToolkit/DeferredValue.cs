// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IQToolkit
{
    public struct DeferredValue<T> : IDeferLoadable
    {
        IEnumerable<T> source;
        bool loaded;
        T value;

        public DeferredValue(T value)
        {
            this.value = value;
            this.source = null;
            this.loaded = true;
        }

        public DeferredValue(IEnumerable<T> source)
        {
            this.source = source;
            this.loaded = false;
            this.value = default(T);
        }

        public void Load()
        {
            if (this.source != null)
            {
                this.value = this.source.SingleOrDefault();
                this.loaded = true;
            }
        }

        public bool IsLoaded
        {
            get { return this.loaded; }
        }

        public bool IsAssigned
        {
            get { return this.loaded && this.source == null; }
        }

        private void Check()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }
        }

        public T Value
        {
            get
            {
                this.Check();
                return this.value;
            }

            set
            {
                this.value = value;
                this.loaded = true;
                this.source = null;
            }
        }
    }
}