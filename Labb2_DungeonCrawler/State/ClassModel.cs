using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.State
{
    internal class ClassModel
    {
        public ObjectId Id { get; set; }
        public string ClassName { get; set; }
    }
}
