--------------------------------------------------------------
-- simple script that sets something to be invisible as soon as it loads

-- created by brandi... 2/4/11
--------------------------------------------------------------

function onRenderComponentReady(self,msg)
	self:SetVisible{visible = false, fadeTime = 0}
end