
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal
{ 
    public abstract class RefCountedObject
    {
        private int ReferenceCount;

        public void Retain()
        {
            ReferenceCount++;
        }

        public void Release()
        {
            ReferenceCount--;
            if (ReferenceCount == 0)
            {
                Dispose();
            }
        }

        public abstract void Dispose();
    }
}