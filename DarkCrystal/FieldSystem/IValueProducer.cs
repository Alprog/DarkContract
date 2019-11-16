
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.FieldSystem
{
    public interface IValueProducer<T>
    {
        T Produce(Entity context);
    }
}