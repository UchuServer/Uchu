using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class ScriptComponentRow
    {
        [Column("id")]
        public int ScriptId { get; set; }
        
        [Column("script_name")]
        public string ServerScript { get; set; }
        
        [Column("client_script_name")]
        public string ClientScript { get; set; }
    }
}