using System;

namespace Aleria.Excp
{
    class FileBaseKnowledgeNotFound  :Exception
    {
        public FileBaseKnowledgeNotFound() : base() { }

        public FileBaseKnowledgeNotFound(String message) : base("File base knowledge tidak dapat ditemukan : " + message) { } 
    }
}
