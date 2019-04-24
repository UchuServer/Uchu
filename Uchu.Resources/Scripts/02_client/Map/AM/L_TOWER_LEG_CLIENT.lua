--------------------------------------------------------------
-- Client Zone script the legs on the watchtowers in the skeleton area of Aura Mar

-- created brandi... 12/8/10 
--------------------------------------------------------------
require('02_client/Map/General/L_SPINJITZU_REQUIRED_ICON')

-- when the asset is done rendering
function onRenderComponentReady(self,msg)
	-- turn the health bar off
	self:SetNameBillboardState{bState = false }
	-- set them to not be hit able from too far away.
	self:OverrideBoundingRadius{ fBoundingRadius = 2}
end 