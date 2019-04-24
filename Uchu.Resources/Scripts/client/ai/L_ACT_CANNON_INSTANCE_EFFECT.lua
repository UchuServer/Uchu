require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//  Client side script for GF SG Effects
--//  - Hides the effect actors
--///////////////////////////////////////////////////////////////////////////////////////

--------------------------------------------------------------
-- Called when rendering is complete for this object
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

	-- instant hide of object	
	self:SetVisible { visible = false, fadeTime = 0 }

end

