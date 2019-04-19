using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class ScriptComponentRow
    {
        [Column("id")]
        public int ScriptId { get; set; }
        
        [Column("script_name")]
        public string ServerScriptName { get; set; }
        
        [Column("client_script_name")]
        public string ClientScriptName { get; set; }
    }
}