-----------------------------------------------------------------------
-- This script is attached to the [client] finish line quickbuild in AG.
-- It handles the cinematic when the player builds the finish line and 
-- then shows the start/finish line objects.
--
-- updated mrb... 7/21/11 - check that it's the right player
-----------------------------------------------------------------------

function onRebuildNotifyState(self, msg)
    if msg.iState ~= 2 then return end
    
	msg.player:PlayCinematic{pathName = "Mon_FinishLine_2"}		
	self:SetVar("completedBuilder", msg.player:GetID())
end 

function onDie(self, msg)
	if not self:GetVar("completedBuilder") or GAMEOBJ:GetControlledID():GetID() ~= self:GetVar("completedBuilder") then return end
	
	-- show everything in the "StartLine" group
	local obj = self:GetObjectsInGroup{ group = "StartLine", ignoreSpawners = true }.objects
	
	for k,v in ipairs(obj) do
		v:SetVisible{visible = true, fadeTime = 0.1}
		v:SetCollisionGroupToOriginal()
	end
end 