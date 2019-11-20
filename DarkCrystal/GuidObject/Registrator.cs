
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal
{
    public class Registrator
    {
        public static bool SetObject<T>(T guidObject, bool state) where T : GuidObject
        {
            if (state)
            {
                GuidStorage<T>.Internal.Register(guidObject.Guid, guidObject);
            }
            else
            {
                GuidStorage<T>.Internal.Unregister(guidObject.Guid);
            }
            return true;
        }
    }
}