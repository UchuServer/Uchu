--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

function onStartup(self) 
	
	       GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "startup", self )
			self:SetNetworkVar("broken", true )
			self:SetFaction{faction = 101}
		
			--self:Die{ killerID = self }
		    
end


function onRebuildComplete(self, msg)

  self:SetNetworkVar("broken", false )
        
end  

onTimerDone = function (self, msg)


	if msg.name == "startup" then
	    
		self:RebuildReset{ bFail = false }
		self:SetNetworkVar("broken", true )
	
		
	end



end

function RebuildComplete(self, msg)
    print("test")
    msg = temp

end

function onHit(self,msg)
    if msg then
    
    end
end

