require('o_mis')

function onStartup(self)
	                       
end

function onRenderComponentReady(self, msg) 

end

function onPCreateEffectFinished(self, msg)

    self:PlayFXEffect{ effectType="onspawn" }	

end

--------------------------------------------------------------
-- handle proximity updates
--------------------------------------------------------------
function onProximityUpdate(self, msg)

	if msg.status == "ENTER" then 

        -- forward the event to the parent
        if (msg.objId:GetLOT().objtemplate == 3710) then
            getParent(self):FireEvent{args = "monkey_prox", senderID = self}
        end
		
	end

end
