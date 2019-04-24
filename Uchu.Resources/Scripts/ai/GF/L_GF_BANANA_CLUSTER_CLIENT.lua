--------------------------------------------------------------
-- Script on the banana clusters in Gnarled Forest Client Side to 
-- handle renderupdates.
-- 
-- updated Lucas Utterback... 5/10/10
--------------------------------------------------------------

function onRenderComponentReady(self, msg)
	self:SetUpdatable{bUpdatable=true};
end