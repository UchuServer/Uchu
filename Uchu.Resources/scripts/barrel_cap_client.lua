require('o_mis')



function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end






--------------------------------------------------------------
-- Called when rendering is complete for this object
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 



	
	self:SetColor{iLEGOColorID =192}


end



