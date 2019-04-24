--------------------------------------------------------------
-- Locks the base of the turret so it can rotate with a stable base.
-- MEdwards 11/12/10

--------------------------------------------------------------

function onRenderComponentReady(self, msg) 
	self:LockNodeRotation{nodeName = "base"}
end
