--L_ACT_CANNONBALL.lua
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

-- Ian G. There is now a detructible component attached to this object so we do not handle
-- get faction or isenemy in lua anymore.
--------------------------------------------------------------
-- return the parent's faction
--------------------------------------------------------------
--function onGetFaction(self, msg)
--	msg.faction = self:GetVar("My_Faction")
--end
--------------------------------------------------------------
-- Determine if the target is an enemy
--------------------------------------------------------------
--function onIsEnemy(self, msg)

-- get our faction from our parent
--	local myFaction = self:GetVar("My_Faction")
-- get the target's faction
--	local tgt = msg.targetID;
--	local tgtFaction = tgt:GetFaction().faction
-- target is an enemy if the faction is not the same as us
--	msg.enemy = (myFaction ~= tgtFaction)
--	return msg
--end


function onStartup(self)


	self:AddObjectToGroup{ group = "cannonball" }


end
