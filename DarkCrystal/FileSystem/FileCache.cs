
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;

namespace DarkCrystal
{
    public class FileCache
    {
        public readonly DateTime LastWriteTime;
        private byte[] m_bytes;
        private string m_text;

        public FileCache(DateTime lastWriteTime, byte[] bytes = null, string text = null)
        {
            this.LastWriteTime = lastWriteTime;
            this.m_bytes = bytes;
            this.m_text = text;
        }

        public byte[] Bytes
        {
            get
            {
                if (m_bytes == null)
                {
                    m_bytes = Encoding.UTF8.GetBytes(m_text);
                }
                return m_bytes;
            }
        }

        public string Text
        {
            get
            {
                if (m_text == null)
                {
                    m_text = Encoding.UTF8.GetString(m_bytes);
                }
                return m_text;
            }
        }
    }
}