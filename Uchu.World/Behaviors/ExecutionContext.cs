using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    using SyncDelegate = Func<BitReader, Task>;
    
    public class ExecutionContext
    {
        public readonly List<GameObject> Targets = new List<GameObject>();

        public readonly Dictionary<string, Delay> Delays = new Dictionary<string, Delay>();

        public readonly GameObject Associate;

        public BehaviorBase Root { get; set; }

        public BitReader Reader { get; set; }
        
        public readonly Dictionary<uint, SyncDelegate> BehaviorHandles = new Dictionary<uint, SyncDelegate>();
         
        public ExecutionContext(GameObject associate, BitReader reader)
        {
            Associate = associate;
            Reader = reader;
        }

        public async Task SyncAsync(uint handle, BitReader reader)
        {
            if (BehaviorHandles.TryGetValue(handle, out var method))
            {
                await method(reader);

                BehaviorHandles.Remove(handle);
            }
        }

        public string DebugGraph()
        {
            return DebugNode(Root);
        }

        private string DebugNode(BehaviorBase node)
        {
            var str = new StringBuilder();
            
            str.AppendLine($"{GetDepthTab(node)}[{node.BehaviorId}] {node.Id} ->");

            foreach (var childNode in node.ChildNodes)
            {
                str.AppendLine(DebugNode(childNode));
            }

            return str.ToString();
        }

        private string GetDepthTab(BehaviorBase node)
        {
            if (node.ParentNode.Id == BehaviorTemplateId.Empty) return "";
            
            var depth = 0;
            
            var parent = node.ParentNode;

            while (parent.Id != BehaviorTemplateId.Empty)
            {
                depth++;

                parent = parent.ParentNode;
            }
            
            return string.Concat(Enumerable.Repeat('\t', depth));
        }
    }
}