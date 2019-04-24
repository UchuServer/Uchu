--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

function onStartup(self) 
	--self:RebuildReset{ bFail = true }
	--GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "startup", self )
   self:SetVar("Built", false)
end


function onRebuildComplete(self, msg)

    self:SetVar("Built", true)
 
end  


onTimerDone = function (self, msg)


	


end

function onRebuildNotifyState(self, msg)

    if msg.iState == 4 then
     self:SetVar("Built", false)
    end
    

end


