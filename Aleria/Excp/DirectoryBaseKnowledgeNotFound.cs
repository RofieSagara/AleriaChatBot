using System;

namespace Aleria.Excp
{
    public class DirectoryBaseKnowledgeNotFound : Exception
    {
        public DirectoryBaseKnowledgeNotFound() :base() { }

        public DirectoryBaseKnowledgeNotFound(String message) : base("Directory tidak dapat di temukan : "+ message) { }
    }
}
