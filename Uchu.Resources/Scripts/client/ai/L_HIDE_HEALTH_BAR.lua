--------------------------------------------------------------
-- Client side script to hide the health bar above an object

-- updated Brandi... 2/25/10
--------------------------------------------------------------
function onRenderComponentReady(self)

	--turn off the health bar above object
	self:SetNameBillboardState{bState = false }
	self:SetVar("smashed",false)
	
end