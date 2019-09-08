require('o_mis')
function onStartup(self) 

end


function onCollisionPhantom(self, msg)
   
    local target = msg.senderID
   
   	local lot = target:GetLOT{}.objtemplate
 
    
    if (lot == 4975) then
    	target:Die{ killerID = target, killType = "SILENT" }
    end
    
end
